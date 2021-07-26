using System;
using System.IO;
using System.ServiceModel;
using FileStorage.Contracts;
using Mindscan.Media.Utils.Logger;
using Mindscan.Media.Utils.Retry;
using Mindscan.Media.VideoUtils.Config;
using Service.Registry.Common;

namespace Mindscan.Media.VideoUtils.FileStorage.Impl
{
	internal class FileStorageFacade : IFileStorageFacade
	{
		private readonly ILogger _logger;
		private readonly IServiceRegistryFactory _serviceRegistryFactory;
		private readonly IVideoUtilsConfig _videoUtilsConfig;
		private readonly IRetryManager _retry;
		private const int AllowedFileSizeDiff = 8;
		private const int AllowedFileSizeDiffPercent = 50;
		private const int BORDER_FILE_SIZE = 1800000000;

		public Uri FileStorageUrl { get; } = new Uri("http://files.mindscan.ru");

		public FileStorageFacade(
			IServiceRegistryFactory serviceRegistryFactory,
			ILoggerFactory loggerFactory,
			IVideoUtilsConfig videoUtilsConfig,
			IRetryBuilder retryBuilder
		)
		{
			_serviceRegistryFactory = serviceRegistryFactory;
			_videoUtilsConfig = videoUtilsConfig;
			_logger = loggerFactory.CreateLogger(GetType().Name);
			_retry = retryBuilder
					.For<CommunicationException>()
					.Retry(videoUtilsConfig.UploadToFileStorageRetryInterval, videoUtilsConfig.RetryCount);
		}

		private string UploadIfNotExist(string path)
		{
			if (_videoUtilsConfig.Debug)
			{
				return path;
			}

			var fileName = Path.GetFileName(path);
			var proxy = _serviceRegistryFactory.CreateProxy<IWcfFileStorageService>();
			using ((IClientChannel)proxy)
			{
				var id = proxy.GetExistIdByExternal(fileName);

				if (!string.IsNullOrWhiteSpace(id))
				{
					var remoteSize = proxy.GetSize(id);
					var localSize = new FileInfo(path).Length;

					if (Math.Abs(remoteSize - localSize) < AllowedFileSizeDiff)
					{
						return id;
					}
					_logger.Warn($"The size of {path} and remote file do not match. The file will be reloaded.");
				}

				var file = new FileInfo(path);
				if (file.Length > BORDER_FILE_SIZE)
				{
					return UploadLargeFile(path, fileName, proxy);
				}

				return UploadFile(path, fileName, proxy);
			}
		}

		private string UploadIfNotExistByBytes(string fileName, byte[] bytes)
		{
			if (_videoUtilsConfig.Debug)
			{
				return fileName;
			}

			var proxy = _serviceRegistryFactory.CreateProxy<IWcfFileStorageService>();
			using ((IClientChannel)proxy)
			{
				var id = proxy.GetExistIdByExternal(fileName);

				if (!string.IsNullOrWhiteSpace(id))
					return id;

				var fileInfoResponse = proxy.SaveBytesByExternal(bytes, fileName);

				return fileInfoResponse.Id;
			}
		}

		public string CheckFile(string fileName, long localSize)
		{
			if (_videoUtilsConfig.Debug)
			{
				return string.Empty;
			}

			var proxy = _serviceRegistryFactory.CreateProxy<IWcfFileStorageService>();
			using ((IClientChannel)proxy)
			{
				var id = proxy.GetExistIdByExternal(fileName);

				if (!string.IsNullOrWhiteSpace(id))
				{
					var remoteSize = proxy.GetSize(id);

					_logger.Info($"Checking {fileName} with id {id}: remoteSize - {remoteSize}, localSize - {localSize}");

					if (Math.Abs(1 - (double)remoteSize / localSize) * 100 < AllowedFileSizeDiffPercent)
					{
						return id;
					}
				}
			}

			return string.Empty;
		}

		private static string UploadFile(string path, string fileName, IWcfFileStorageService proxy)
		{
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				using (var batch = new FileStorageBatchPersist(proxy, fileName))
				{
					return batch.Write(stream);
				}
			}
		}

		private static string UploadLargeFile(string path, string fileName, IWcfFileStorageService proxy)
		{
			using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				var id = "";

				var length = 11000;
				var offset = 0;

				var byteCount = 0;

				var buffer = new byte[length];
				while ((byteCount = stream.Read(buffer, 0, buffer.Length)) > 0)
				{
					bool close = length > byteCount || offset + byteCount > stream.Length;
					var send = new byte[byteCount];
					Array.Copy(buffer, send, byteCount);

					var data = proxy.SaveByName(path, send, close);
					id = data.Id;
					offset += length;
				}

				return id;
			}
		}

		public Uri SendMediaFileToFileStorage(string localPath)
		{
			if (_videoUtilsConfig.Debug)
			{
				return new Uri($"file:///{localPath}");
			}
			_logger.Info($"Started sending file {localPath}... ");
			var uuid = _retry.InvokeWithRetry(() => UploadIfNotExist(localPath));
			_logger.Info($"File {localPath} sent to storage with uid {uuid}");
			return new Uri(FileStorageUrl, uuid);
		}

		public virtual Uri UploadFromBase64String(string imgBase64, string name)
		{
			if (_videoUtilsConfig.Debug)
			{
				return new Uri($"file:///{name}");
			}

			var bytes = Convert.FromBase64String(imgBase64);
			var uuid = _retry.InvokeWithRetry(() => UploadIfNotExistByBytes(name, bytes));
			if (uuid != null)
			{
				_logger.Info($"File {name} uploaded to storage uid = {uuid}.");
				return new Uri(FileStorageUrl, uuid);
			}
			_logger.Error($"CommunicationException while trying to download the file {name}.");

			return null;
		}

		public void Dispose()
		{
		}
	}
}
