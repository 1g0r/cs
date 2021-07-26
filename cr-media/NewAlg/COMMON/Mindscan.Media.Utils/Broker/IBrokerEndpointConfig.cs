using System;
using Mindscan.Media.Utils.Config;

namespace Mindscan.Media.Utils.Broker
{
	public interface IBrokerEndpointConfig : IConfig
	{
		string Name { get; }
		string Host { get; }
		string UserName { get; }
		string Password { get; }
		string VirtualHost { get; }
		TimeSpan ConnectionTimeout { get; }
		TimeSpan RecoveryInterval { get; }
		TimeSpan RetryWaitInterval { get; }
		ushort PrefetchCount { get; }
		ushort ConsumersCount { get; }
		int MaxMessageSize { get; }
	}
}