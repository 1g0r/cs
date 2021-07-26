using System;
using System.Collections.Generic;
using Mindscan.Media.Bundles.New.Helpers;
using Mindscan.Media.Utils.Config.Impl;
using Mindscan.Media.VideoUtils.DiskCleaner;
using Mindscan.Media.VideoUtils.DiskCleaner.Impl;
using Mindscan.Media.VideoUtils.VideoConverter;
using Mindscan.Media.VideoUtils.VideoConverter.Impl;

namespace Mindscan.Media.VideoUtils.Config.Impl
{
	internal sealed class VideoUtilsConfig : ConfigBase, IVideoUtilsConfig
	{
		public IVideoConverterConfig VideoConverterConfig { get; private set; }
		public IDiskCleanerConfig DiskCleanerConfig { get; private set; }
		public TimeSpan UploadToFileStorageRetryInterval { get; private set; }
		public int RetryCount { get; private set; }
		public Dictionary<string, string> EncodingOptions { get; private set; }
		public string ScreenShotOptions { get; private set; }
		public int SecondsToScreenShot { get; private set; }
		public TimeSpan GetScreenshotTimeout { get; private set; }
		public TimeSpan ConvertVideoTimeout { get; private set; }

		public VideoUtilsConfig(string sectionName) : base(sectionName)
		{
		}

		protected override void BuildConfig()
		{
			GetScreenshotTimeout = GetAttributeValueAndCast<TimeSpan>(nameof(GetScreenshotTimeout), TimeSpan.TryParse, "00:00:15");
			ConvertVideoTimeout = GetAttributeValueAndCast<TimeSpan>(nameof(ConvertVideoTimeout), TimeSpan.TryParse, "00:40:00");

			VideoConverterConfig = new VideoConverterConfig(Raw?.Element(nameof(VideoConverter)));
			DiskCleanerConfig = new DiskCleanerConfig(Raw?.Element(nameof(DiskCleaner)));
			RetryCount = GetAttributeValueAndCast<int>(nameof(RetryCount), int.TryParse, "15");
			UploadToFileStorageRetryInterval = GetAttributeValueAndCast<TimeSpan>(nameof(UploadToFileStorageRetryInterval), TimeSpan.TryParse, "00:40:00");

			SecondsToScreenShot = GetAttributeValueAndCast<int>(
				nameof(SecondsToScreenShot),
				int.TryParse,
				"2");

			ScreenShotOptions = GetAttributeValue(nameof(ScreenShotOptions), false);

			EncodingOptions = new Dictionary<string, string>();
			Raw?.Element(nameof(EncodingOptions))?.Elements("option").ForEach(x =>
			{
				var key = x.Attribute("extension")?.Value;
				var value = x.Attribute("options")?.Value;
				if (!key.IsNullOrEmpty() && !value.IsNullOrEmpty())
				{
					EncodingOptions.Add(key, value);
				}
			});
		}
	}
}