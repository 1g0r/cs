using System.Xml.Linq;
using Mindscan.Media.Utils.Config.Impl;

namespace Mindscan.Media.Utils.Logger.Impl
{
	internal sealed class LoggerConfig: ConfigBase, ILoggerConfig
	{
		public LoggerConfig(XElement sectionXml) : base(sectionXml)
		{
		}
		protected override void BuildConfig()
		{
			ConfigPath = GetAttributeValue(nameof(ConfigPath), "logger.config");
			ErrorFilePath = GetAttributeValue(nameof(ErrorFilePath), false);
			DebugFilePath = GetAttributeValue(nameof(DebugFilePath), false);
		}

		public string ConfigPath { get; private set; }
		public string ErrorFilePath { get; private set; }
		public string DebugFilePath { get; private set; }
	}
}