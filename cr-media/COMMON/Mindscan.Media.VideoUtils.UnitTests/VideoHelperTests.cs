using System.IO;
using FluentAssertions;
using Mindscan.Media.Utils.Logger;
using Mindscan.Media.VideoUtils.Config;
using Mindscan.Media.VideoUtils.VideoConverter;
using Mindscan.Media.VideoUtils.VideoConverter.Impl;
using Moq;
using NUnit.Framework;

namespace Mindscan.Media.VideoUtils.UnitTests
{
	[TestFixture]
	public class VideoHelperTests
	{
		private const string AcceptedVideoFormats = "h264 (Main) (avc1 / 0x31637661);h264 (High) (avc1 / 0x31637661);h264 (Constrained Baseline) (avc1 / 0x31637661);h264 (Baseline) (avc1 / 0x31637661)";
		private const string AcceptedAudioFormats = "aac (LC) (mp4a / 0x6134706D);aac (HE-AAC) (mp4a / 0x6134706D)";

		private IVideoHelper _sut;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			var loggerFactoryMock = new Mock<ILoggerFactory>();
			loggerFactoryMock
				.Setup(lf => lf.CreateLogger(It.IsAny<string>()))
				.Returns(new Mock<ILogger>().Object);

			var videoConfig = new Mock<IVideoConverterConfig>();
			videoConfig
				.Setup(vc => vc.AcceptedAudioFormats)
				.Returns(AcceptedAudioFormats.Split(';'));
			videoConfig
				.Setup(vc => vc.AcceptedVideoFormats)
				.Returns(AcceptedVideoFormats.Split(';'));

			var config = new Mock<IVideoUtilsConfig>();
			config
				.Setup(c => c.VideoConverterConfig)
				.Returns(videoConfig.Object);

			_sut = new VideoHelper(config.Object, loggerFactoryMock.Object);
		}

		[TestCase("video_not_ok.avi")]
		public void VideoNeedConvet(string file)
		{
			//
			var fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, file);

			//
			var result = _sut.IsConversionRequired(fileName);

			//
			result.Should().BeTrue();
		}

		[TestCase("video_ok.mp4")]
		public void VideoDontNeedConvet(string file)
		{
			//
			var fileName = Path.Combine(TestContext.CurrentContext.TestDirectory, file);

			//
			var result = _sut.IsConversionRequired(fileName);

			//
			result.Should().BeFalse();
		}
	}
}