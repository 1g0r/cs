using System;
using System.Collections.Generic;
using Mindscan.Media.Utils.Config;
using Mindscan.Media.VideoUtils.DiskCleaner;
using Mindscan.Media.VideoUtils.VideoConverter;

namespace Mindscan.Media.VideoUtils.Config
{
	public interface IVideoUtilsConfig : IConfig
	{
		IVideoConverterConfig VideoConverterConfig { get; }
		IDiskCleanerConfig DiskCleanerConfig { get; }
		TimeSpan UploadToFileStorageRetryInterval { get; }
		int RetryCount { get; }
		Dictionary<string, string> EncodingOptions { get; }
		string ScreenShotOptions { get; }
		int SecondsToScreenShot { get; }
		TimeSpan GetScreenshotTimeout { get; }
		TimeSpan ConvertVideoTimeout { get; }
	}
}