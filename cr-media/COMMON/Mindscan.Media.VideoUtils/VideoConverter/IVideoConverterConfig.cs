using System.Collections.Generic;
using Mindscan.Media.Utils.Config;

namespace Mindscan.Media.VideoUtils.VideoConverter
{
	public interface IVideoConverterConfig : IConfig
	{
		string EncoderPath { get; }
		IEnumerable<string> AcceptedVideoFormats { get; }
		IEnumerable<string> AcceptedAudioFormats { get; }
	}
}