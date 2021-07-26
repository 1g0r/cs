using System.Collections.Generic;
using Mindscan.Media.Utils.Config;

namespace Mindscan.Media.Utils.Broker
{
	public interface IMessageBrokerConfig : IConfig
	{
		Dictionary<string, IBrokerEndpointConfig> Endpoints { get; }
	}
}