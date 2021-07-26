using Mindscan.Media.Utils.Broker;
using Mindscan.Media.Utils.Broker.Impl;
using Mindscan.Media.Utils.Host;
using Mindscan.Media.Utils.Host.Impl;
using Mindscan.Media.Utils.Logger;
using Mindscan.Media.Utils.Logger.Impl;
using Mindscan.Media.Utils.Proxy;
using Mindscan.Media.Utils.Proxy.Impl;

namespace Mindscan.Media.Utils.Config.Impl
{
	internal sealed class UtilsConfig : ConfigBase, IUtilsConfig
	{
		public IProxyRepositoryConfig ProxyRepositoryConfig { get; private set; }
		public ILoggerConfig LoggerConfig { get; private set; }
		public IHostConfig HostConfig { get; private set; }
		public IMessageBrokerConfig BrokerConfig { get; private set; }

		public UtilsConfig(string sectionName) : base(sectionName)
		{
		}

		protected override void BuildConfig()
		{
			ProxyRepositoryConfig = new ProxyRepositorySettings(Raw?.Element(nameof(ProxyRepositoryConfig)));
			LoggerConfig = new LoggerConfig(Raw?.Element(nameof(LoggerConfig)));
			HostConfig = new HostConfig(Raw?.Element(nameof(HostConfig)));
			BrokerConfig = new MessageBrokerConfig(Raw?.Element(nameof(BrokerConfig)));
		}
	}
}