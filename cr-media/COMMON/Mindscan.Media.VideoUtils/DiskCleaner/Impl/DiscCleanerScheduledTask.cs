using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using Mindscan.Media.Utils.Logger;
using Mindscan.Media.Utils.Scheduler;
using Mindscan.Media.VideoUtils.Config;

namespace Mindscan.Media.VideoUtils.DiskCleaner.Impl
{
	internal class DiscCleanerScheduledTask : IScheduledTask
	{
		private const long GB = 1024 * 1024 * 1024;
		private readonly string _rootDirectory;
		private readonly int _gigaBytesToKeepOnRoot;
		private readonly ILogger _logger;

		public DiscCleanerScheduledTask(
			IVideoUtilsConfig config,
			ILoggerFactory loggerFactory
		)
		{
			var diskCleanerConfig = config.DiskCleanerConfig;
			if (diskCleanerConfig == null)
				throw new ConfigurationErrorsException("DiscCleaner");
			_rootDirectory = diskCleanerConfig.RootDirectory;
			_gigaBytesToKeepOnRoot = diskCleanerConfig.GBytesToKeepOnRoot;
			_logger = loggerFactory.CreateLogger(GetType().Name);
		}

		public void Execute(CancellationToken token)
		{
			int counter = 0;
			if (!Directory.Exists(_rootDirectory))
			{
				_logger.Warn($"Directory {_rootDirectory} no exists");
				return;
			}

			while (!token.IsCancellationRequested && GetDirectorySizeInGb() > _gigaBytesToKeepOnRoot)
			{
				var oldFile = Directory.EnumerateFiles($"{_rootDirectory}", "*", SearchOption.AllDirectories)
					.Select(name => new FileInfo(name))
					.OrderBy(f => f.LastWriteTime)
					.ToList()
					.FirstOrDefault();

				oldFile?.Delete();
				_logger.Info($"File {oldFile.Name} deleted.");
				counter++;
			}

			_logger.Info($"Выполнена очистка папки {_rootDirectory}. Всего удалено файлов: {counter}.");
		}

		private int GetDirectorySizeInGb()
		{
			var files = Directory.EnumerateFiles($"{_rootDirectory}", "*", SearchOption.AllDirectories);
			var gigaBytes = files.Select(name => new FileInfo(name)).Select(info => info.Length).Sum() / GB;
			return (int)gigaBytes;
		}
	}
}