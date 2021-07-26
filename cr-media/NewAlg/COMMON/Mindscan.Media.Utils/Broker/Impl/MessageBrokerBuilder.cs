using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Threading;
using Mindscan.Media.Utils.Config;
using Mindscan.Media.Utils.IoC.Impl;
using Mindscan.Media.Utils.IoC;
using Mindscan.Media.Utils.Logger;
using Mindscan.Media.Utils.Retry;

namespace Mindscan.Media.Utils.Broker.Impl
{
	internal class MessageBrokerBuilder : IMessageBrokerBuilder
	{
		private readonly IMessageBrokerConfig _config;
		private readonly ILoggerFactory _factory;
		private readonly IRetryBuilder _retryBuilder;
		private static readonly ConcurrentDictionary<string, Lazy<IMessageBroker>> _connections = 
			new ConcurrentDictionary<string, Lazy<IMessageBroker>>();

		public MessageBrokerBuilder(IUtilsConfig config, ILoggerFactory factory, IRetryBuilder retryBuilder)
		{
			_config = config.BrokerConfig;
			_factory = factory;
			_retryBuilder = retryBuilder;
		}

		IMessageBroker IMessageBrokerBuilder.Build(CancellationToken token, string endpointName)
		{
			IBrokerEndpointConfig config;
			if (_config.Endpoints.TryGetValue(endpointName.ToLower(), out config))
			{
				return _connections.GetOrAdd(
					endpointName.ToLower(),
					key => new Lazy<IMessageBroker>(() => CreateConnection(key, config, token))).Value;
			}
			throw new ConfigurationErrorsException($"Unable to find broker configuration for endpoint {endpointName}");
		}

		public void Dispose()
		{
			foreach (var connection in _connections.Values)
			{
				connection?.Value?.Dispose();
			}
			_connections.Clear();
		}

		private IMessageBroker CreateConnection(string name, IBrokerEndpointConfig config, CancellationToken token)
		{
			var result = new BrokerConnection(_factory, config, _retryBuilder, token);
			Dependency.Registrar.Register<IMessageBroker>(name, result, Lifetime.Singleton);
			return result;
		}
	}
}