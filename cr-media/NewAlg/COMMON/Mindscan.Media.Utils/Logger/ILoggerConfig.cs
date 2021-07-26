using Mindscan.Media.Utils.Config;

namespace Mindscan.Media.Utils.Logger
{
	public interface ILoggerConfig : IConfig
	{
		string ConfigPath { get; }
		string ErrorFilePath { get; }
		string DebugFilePath { get; }
	}
}