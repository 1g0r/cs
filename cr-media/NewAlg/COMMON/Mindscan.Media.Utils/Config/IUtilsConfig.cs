using Mindscan.Media.Utils.Broker;
using Mindscan.Media.Utils.Host;
using Mindscan.Media.Utils.Logger;
using Mindscan.Media.Utils.Proxy;

namespace Mindscan.Media.Utils.Config
{
	public interface IUtilsConfig : IConfig
	{
		IProxyRepositoryConfig ProxyRepositoryConfig { get; }
		ILoggerConfig LoggerConfig { get; }
		IHostConfig HostConfig { get; }
		IMessageBrokerConfig BrokerConfig { get; }
	}
}