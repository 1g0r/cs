using IVideoUtilsConfig = Mindscan.Media.VideoUtils.Config.IVideoUtilsConfig;

namespace Mindscan.Media.VideoUtils.VideoConverter.Impl
{
	internal sealed class VideoConverterFactory
	{
		private readonly IVideoConverterConfig _converterSettings;
		public VideoConverterFactory(IVideoUtilsConfig config)
		{
			_converterSettings = config.VideoConverterConfig;
		}
		public VideoConverter GetConverter()
		{
			return new VideoUtils.VideoConverter.Impl.VideoConverter(_converterSettings);
		}
	}
}