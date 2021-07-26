using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Mindscan.Media.Utils.Config.Impl;

namespace Mindscan.Media.VideoUtils.VideoConverter.Impl
{
	internal sealed class VideoConverterConfig : ConfigBase, IVideoConverterConfig
	{
		public VideoConverterConfig(XElement sectionXml) : base(sectionXml)
		{
		}

		protected override void BuildConfig()
		{
			EncoderPath = GetAttributeValue(nameof(EncoderPath));
			AcceptedVideoFormats = GetAttributeValue(nameof(AcceptedVideoFormats)).Split(';').ToList();
			AcceptedAudioFormats = GetAttributeValue(nameof(AcceptedAudioFormats)).Split(';').ToList();
		}

		public string EncoderPath { get; private set; }
		public IEnumerable<string> AcceptedVideoFormats { get; private set; }
		public IEnumerable<string> AcceptedAudioFormats { get; private set; }
	}
}