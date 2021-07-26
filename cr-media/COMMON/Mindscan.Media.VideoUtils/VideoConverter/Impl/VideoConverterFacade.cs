using System;
using System.IO;
using System.Linq;
using System.Threading;
using Mindscan.Media.DomainObjects.Integration;
using Mindscan.Media.Messages.Collector;
using Mindscan.Media.Utils.Logger;
using Mindscan.Media.VideoUtils.Config;
using Mindscan.Media.VideoUtils.FileStorage;

namespace Mindscan.Media.VideoUtils.VideoConverter.Impl
{
	internal class VideoConverterFacade : IVideoConverterFacade
	{
		private const int MB = 1024 * 1024;
		private const int KB = 1024;
		private ILogger _logger;
		private readonly VideoConverterFactory _videoConverterFactory;
		private readonly IVideoUtilsConfig _videoUtilsConfig;
		private readonly IVideoHelper _videoHelper;
		private readonly IFileStorageFacade _facade;

		public VideoConverterFacade(
			ILoggerFactory loggerFactory,
			VideoConverterFactory videoConverterFactory,
			IVideoHelper videoHelper,
			IFileStorageFacade facade,
			IVideoUtilsConfig videoUtilsConfig)
		{
			_videoConverterFactory = videoConverterFactory;
			_videoHelper = videoHelper;
			_facade = facade;
			_videoUtilsConfig = videoUtilsConfig;
			_logger = loggerFactory.CreateLogger(GetType().Name);
		}

		public void ConvertVideo(MaterialData material, string localname, CancellationToken cancellationToken)
		{
			var screenshot = GetScreenshot(localname, cancellationToken);

			if (screenshot != null)
			{
				var imageUrl = _facade.SendMediaFileToFileStorage(screenshot.Url.LocalPath);
				if (imageUrl != null)
				{
					material.Images.Add(new LinkData { Url = imageUrl, Title = screenshot.Title });
				}
			}

			if (_videoHelper.IsConversionRequired(localname))
			{
				localname = ConvertVideo(localname, cancellationToken);
			}

			if (localname != "")
			{
				var videoUri = _facade.SendMediaFileToFileStorage(localname);
				material.Videos.Add(new LinkData() { Url = videoUri, Title = Path.GetFileNameWithoutExtension(localname) });
			}
		}

		private string ConvertVideo(string sourceVideoPath, CancellationToken cancellationToken)
		{
			if (_videoUtilsConfig.Debug)
			{
				return sourceVideoPath;
			}

			_logger.Info($"Start encoding process for {sourceVideoPath}");
			var encodingOptions = _videoUtilsConfig.EncodingOptions;

			using (var converter = _videoConverterFactory.GetConverter())
			{
				string convertOptions;
				encodingOptions.TryGetValue(Path.GetExtension(sourceVideoPath), out convertOptions);

				var convertedVideoPath = Path.Combine(
					Path.GetDirectoryName(sourceVideoPath),
					Path.GetFileNameWithoutExtension(sourceVideoPath) + "_converted.mp4");

				if (String.IsNullOrWhiteSpace(convertOptions))
				{
					encodingOptions.TryGetValue("default", out convertOptions);
				}

				if (!String.IsNullOrWhiteSpace(convertOptions))
				{
					var fileSize = new FileInfo(sourceVideoPath).Length / MB;
					var result = cancellationToken.IsCancellationRequested
						? VideoConverterResult.Canceled
						: converter.Convert(
							sourceVideoPath, convertedVideoPath, convertOptions, (int)_videoUtilsConfig.ConvertVideoTimeout.TotalMilliseconds);

					if (result == VideoConverterResult.Success)
					{
						var newFileSize = new FileInfo(convertedVideoPath).Length / MB;

						_logger.Info($"Videofile {convertedVideoPath} sucessfull encoded [Size before: {fileSize} Mb, size after: {newFileSize} Mb]");

						System.IO.File.Delete(sourceVideoPath);

						var newPath = Path.ChangeExtension(sourceVideoPath, "mp4");

						if (!new FileInfo(newPath).Exists)
						{
							System.IO.File.Move(convertedVideoPath, Path.ChangeExtension(sourceVideoPath, "mp4"));
						}
						else
						{
							System.IO.File.Delete(convertedVideoPath);
						}

						return Path.ChangeExtension(sourceVideoPath, "mp4");
					}

					System.IO.File.Delete(convertedVideoPath);

					throw new Exception($"Encode videofile {sourceVideoPath} [Size: {fileSize} Mb] in {_videoUtilsConfig.ConvertVideoTimeout} ended with result - {result}");
				}
			}

			System.IO.File.Delete(sourceVideoPath);

			_logger.Warn($"File didn't converted. Unknown extension - '{Path.GetExtension(sourceVideoPath)}'");

			return "";
		}

		private Image GetScreenshot(string sourceVideoPath, CancellationToken cancellationToken)
		{
			_logger.Info($"Get screenshot for {sourceVideoPath}");
			Image screenshot = null;
			var secondsToScreenshot = _videoUtilsConfig.SecondsToScreenShot;
			var options = _videoUtilsConfig.ScreenShotOptions;

			using (var converter = _videoConverterFactory.GetConverter())
			{
				var screenshotPath = Path.Combine(
					Path.GetDirectoryName(sourceVideoPath),
					Path.GetFileNameWithoutExtension(sourceVideoPath) + ".jpg");

				if (!String.IsNullOrWhiteSpace(options))
				{
					var result = cancellationToken.IsCancellationRequested
						? VideoConverterResult.Canceled
						: converter.GetScreenshot(
							secondsToScreenshot, sourceVideoPath, screenshotPath, options, (int)_videoUtilsConfig.GetScreenshotTimeout.TotalMilliseconds);

					if (result == VideoConverterResult.Success && new FileInfo(screenshotPath).Exists)
					{
						screenshot = new Image()
						{
							Url = new Uri(screenshotPath),
							Title = Path.GetFileNameWithoutExtension(sourceVideoPath),
							Type = "image/jpeg"
						};

						var newFileSize = new FileInfo(screenshotPath).Length / KB;

						_logger.Info($"Screenshot {screenshotPath} sucessfull recieved. Size: {newFileSize} Kb]");
					}
					else
					{
						_logger.Error($"Attempt to recieve screenshot for {sourceVideoPath} in {_videoUtilsConfig.GetScreenshotTimeout} ended with result - {result}");
					}
				}
			}

			return screenshot;
		}
	}
}