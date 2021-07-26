using System.IO;
using System.Linq;
using MediaToolkit;
using MediaToolkit.Model;
using Mindscan.Media.Utils.Helpers;
using Mindscan.Media.Utils.Logger;
using Mindscan.Media.VideoUtils.Config;

namespace Mindscan.Media.VideoUtils.VideoConverter.Impl
{
	internal class VideoHelper : IVideoHelper
	{
		private readonly IVideoConverterConfig _config;
		private readonly ILogger _logger;

		public VideoHelper(IVideoUtilsConfig config, ILoggerFactory loggerFactory)
		{
			_config = config.VideoConverterConfig;
			_logger = loggerFactory.CreateLogger(GetType().Name);
		}

		public bool IsConversionRequired(string localPath)
		{
			var file = GetFileMetaInfo(localPath);
			if (file.Metadata == null)
				throw new InvalidDataException($"Wrong media file {localPath}.");

			if(file.Metadata.VideoData == null || file.Metadata.VideoData.Format.IsNullOrWhiteSpace())
				throw new InvalidDataException($"Video format of file '{localPath}' is empty.");

			if (file.Metadata.AudioData == null || file.Metadata.AudioData.Format.IsNullOrWhiteSpace())
				throw new InvalidDataException($"Audio format of file '{localPath}' is empty.");

			var isNotAcceptedVideo = !_config.AcceptedVideoFormats.Contains(file.Metadata.VideoData.Format);
			if (isNotAcceptedVideo)
				_logger.Warn($"{file.Filename} video codec '{file.Metadata.VideoData?.Format ?? "null"}' not allowed.");

			var isNotAcceptedAudio = !_config.AcceptedAudioFormats.Contains(file.Metadata.AudioData.Format);
			if (isNotAcceptedAudio)
				_logger.Warn($"{file.Filename} audio codec '{file.Metadata.AudioData?.Format ?? "null"}' not allowed.");

			return isNotAcceptedAudio || isNotAcceptedVideo;
		}

		private MediaFile GetFileMetaInfo(string filename)
		{
			if (!System.IO.File.Exists(filename))
				throw new InvalidDataException("File not exists: " + filename);

			var mediaFile = new MediaFile(filename);
			using (var engine = new Engine())
			{
				engine.GetMetadata(mediaFile);
			}

			return mediaFile;
		}
	}
}