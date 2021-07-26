using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Mindscan.Media.Utils.Broker.Consumer;
using Mindscan.Media.Utils.Logger;
using Mindscan.Media.Utils.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Mindscan.Media.Utils.Broker.Impl
{
	internal class BrokerConnection : IMessageBroker
	{
		private const ushort DEFAULT_CONSUMERS_COUNT = 1;
		private const ushort DEFAULT_PREFETCH_COUNT = 0;

		private readonly ConcurrentDictionary<string, Lazy<SenderChannel>> _senders = new ConcurrentDictionary<string, Lazy<SenderChannel>>();
		private readonly ConcurrentBag<ConsumerChannel> _consumers = new ConcurrentBag<ConsumerChannel>();
		private readonly ConnectionFactory _connectionFactory;
		private readonly object _locker = new object();
		private IConnection _connection;
		private readonly ILogger _logger;
		private readonly ILoggerFactory _loggerFactory;
		private volatile bool _isConnected;
		private readonly string _host;
		private readonly CancellationToken _token;
		private event EventHandler<EventArgs> ConnectionRecovered;
		private event EventHandler<ShutdownEventArgs> ConnectionClosed;
		private readonly IRetryManager _retry;

		internal BrokerConnection(ILoggerFactory factory, IBrokerEndpointConfig config, IRetryBuilder retryBuilder, CancellationToken token)
		{
			_loggerFactory = factory;
			_logger = factory.CreateLogger(GetType().Name);
			_host = $"rabbitmq://{config.Host}/{config.VirtualHost}";
			_token = token;
			Config = config;
			_connectionFactory = new ConnectionFactory
			{
				AutomaticRecoveryEnabled = true,
				HostName = config.Host,
				Password = config.Password,
				RequestedConnectionTimeout = (int)config.ConnectionTimeout.TotalMilliseconds,
				UserName = config.UserName,
				VirtualHost = config.VirtualHost,
				NetworkRecoveryInterval = config.RecoveryInterval,
				RequestedHeartbeat = 60
			};

			_retry = retryBuilder
				.For<AlreadyClosedException>()
				.For<BrokerUnreachableException>()
				.For<OperationInterruptedException>()
				.For<TimeoutException>()
				.For<IOException>()
				.Retry(config.RetryWaitInterval);
		}

		public void Send<T>(string exchange, T message)
		{
			Send(exchange, string.Empty, message);
		}

		public void Send<T>(string exchange, T message, Dictionary<string, string> headers)
		{
			Send(exchange, string.Empty, message, headers);
		}

		public void Send<T>(string exchange, string routingKey, T message)
		{
			Send(exchange, routingKey, message, null);
		}

		public void Send<T>(string exchange, string routingKey, T message, Dictionary<string, string> headers)
		{
			if (string.IsNullOrWhiteSpace(exchange) && string.IsNullOrWhiteSpace(routingKey))
				throw new ArgumentNullException($"Exchange name and routing key can not be empty.");
			_retry.InvokeWithRetry(() =>
			{
				GetSender(exchange, routingKey ?? string.Empty)
					.Send(message, headers);
			});
		}

		public void Publish<T>(T message)
		{
			var type = typeof(T);
			var exchange = $"Exchange:{type.FullName}:{type.Assembly.GetName().Name}";
			Send(exchange, message);
		}

		public IMessageBroker Subscribe<TConsumer>(
			string queue, 
			ushort prefetchCount = DEFAULT_PREFETCH_COUNT, 
			ushort consumersCount = DEFAULT_CONSUMERS_COUNT) where TConsumer : MediaConsumerBase
		{
			return Subscribe<TConsumer>(queue, string.Empty, prefetchCount, consumersCount);
		}

		public IMessageBroker Subscribe<TConsumer>(
			string queue, 
			string exchange, 
			ushort prefetchCount = DEFAULT_PREFETCH_COUNT, 
			ushort consumersCount = DEFAULT_CONSUMERS_COUNT) where TConsumer : MediaConsumerBase
		{
			return Subscribe<TConsumer>(queue, exchange, String.Empty, prefetchCount, consumersCount);
		}

		public IMessageBroker Subscribe<TConsumer>(
			string queue, string exchange, 
			string routingKey, 
			ushort prefetchCount = DEFAULT_PREFETCH_COUNT, 
			ushort consumersCount = DEFAULT_CONSUMERS_COUNT) where TConsumer : MediaConsumerBase
		{
			if (string.IsNullOrWhiteSpace(queue))
				throw new ArgumentNullException(nameof(queue));

			consumersCount = GetConsumersCount(consumersCount);
			
			for (var i = 0; i < consumersCount; ++i)
			{
				GetConsumer(queue, exchange, routingKey)
					.Subscribe<TConsumer>(prefetchCount);
			}
			
			return this;
		}

		public IMessageBroker Subscribe<TMessage, TConsumer>(
			string queue, 
			ushort prefetchCount = DEFAULT_PREFETCH_COUNT, 
			ushort consumersCount = DEFAULT_CONSUMERS_COUNT) 
			where TConsumer : MediaConsumerBase<TMessage>
		{
			return Subscribe<TMessage, TConsumer>(queue, string.Empty, prefetchCount, consumersCount);
		}

		public IMessageBroker Subscribe<TMessage, TConsumer>(
			string queue, 
			string exchange, 
			ushort prefetchCount = DEFAULT_PREFETCH_COUNT, 
			ushort consumersCount = DEFAULT_CONSUMERS_COUNT) 
			where TConsumer : MediaConsumerBase<TMessage>
		{
			return Subscribe<TMessage, TConsumer>(queue, exchange, String.Empty, prefetchCount, consumersCount);
		}

		public IMessageBroker Subscribe<TMessage, TConsumer>(
			string queue, 
			string exchange, 
			string routingKey, 
			ushort prefetchCount = DEFAULT_PREFETCH_COUNT, 
			ushort consumersCount = DEFAULT_CONSUMERS_COUNT) 
			where TConsumer : MediaConsumerBase<TMessage>
		{
			if (string.IsNullOrWhiteSpace(queue))
				throw new ArgumentNullException(nameof(queue));

			consumersCount = GetConsumersCount(consumersCount);

			for (var i = 0; i < consumersCount; ++i)
			{
				GetConsumer(queue, exchange, routingKey)
					.Subscribe<TMessage, TConsumer>(prefetchCount);
			}
			
			return this;
		}

		public Uri Host => new Uri(_host);

		public void Dispose()
		{
			_logger.Debug("Start dispose connection.");
			if (_connection != null)
			{
				_connection.ConnectionShutdown -= OnConnectionShutdown;
				_connection.RecoverySucceeded -= OnRecoverySucceeded;
			}
			ConnectionClosed = null;
			ConnectionRecovered = null;
			foreach (var sender in _senders.Values)
			{
				sender?.Value?.Dispose();
			}

			foreach (var consumer in _consumers)
			{
				consumer?.Dispose();
			}
			_connection?.Close();
			_logger.Debug("Dispose connection.");
			_connection?.Dispose();
			_logger.Debug("Dispose connection done.");
		}

		internal IBrokerEndpointConfig Config { get; }

		internal bool IsAlive
		{
			get
			{
				CreateConnection();
				return !_token.IsCancellationRequested && (_connection?.IsOpen ?? false) && _isConnected;
			}
		}
		internal IModel CreateModel()
		{
			if (IsAlive)
			{
				return _connection.CreateModel();
			}
			return null;
		}

		internal void TrySendError(string queue, BasicDeliverEventArgs args, Exception ex)
		{
			try
			{
				const string errorPostfix = "_Error";
				queue += errorPostfix;
				var exchange = queue;
				GetSender(exchange, "")
					.SendError(queue, args, ex);
			}
			catch (Exception e)
			{
				_logger.Error($"Unable to resend message to error Queue {queue}", e);
			}
		}

		private SenderChannel GetSender(string exchange, string routingKey)
		{
			return _senders.GetOrAdd(
				$"{exchange}{routingKey ?? ""}",
				key => new Lazy<SenderChannel>(() => CreateSender(exchange, routingKey))
			).Value;
		}

		SenderChannel CreateSender(string exchange, string routingKey)
		{
			var sender = new SenderChannel(exchange, routingKey, this, _token, _loggerFactory);
			ConnectionRecovered += sender.OnConnectionRecovered;
			ConnectionClosed += sender.OnConnectionClosed;
			return sender;
		}

		private ConsumerChannel GetConsumer(string queue, string exchange, string routingKey)
		{
			var result = CreateConsumer(queue, exchange, routingKey);
			_consumers.Add(result);
			return result;
		}

		ConsumerChannel CreateConsumer(string queue, string exchange, string routingKey)
		{
			var consumer = new ConsumerChannel(queue, exchange, routingKey, this, _token, _loggerFactory);
			ConnectionRecovered += consumer.OnConnectionRecovered;
			ConnectionClosed += consumer.OnConnectionClosed;
			return consumer;
		}

		private void CreateConnection()
		{
			if (_connection == null)
			{
				lock (_locker)
				{
					if (_connection == null)
					{
						try
						{
							_connection = _connectionFactory.CreateConnection();
							_connection.ConnectionShutdown += OnConnectionShutdown;
							_connection.RecoverySucceeded += OnRecoverySucceeded;
							_isConnected = true;
							_logger.Debug(string.Format(LogMessages.ConnectionEstablished, _host));
						}
						catch
						{
							_isConnected = false;
							_connection?.Dispose();
							_connection = null;
							_logger.Error($"Unable to establish connection to host {_host}.");
							throw;
						}
					}
				}
			}
		}

		private ushort GetConsumersCount(ushort parameterValue)
		{
			if (Config.Debug)
				return 1;
			return parameterValue == DEFAULT_CONSUMERS_COUNT ? 
				Config.ConsumersCount : 
				parameterValue;
		}

#region Event Handlers   
		private void OnConnectionShutdown(object sender, ShutdownEventArgs shutdownEventArgs)
		{
			lock (_locker)
			{
				_isConnected = false;
			}
			_logger.DebugFormat(LogMessages.ConnectionShutdown, _host);
			var events = ConnectionClosed;
			try
			{
				events?.Invoke(sender, shutdownEventArgs);
			}
			catch (Exception ex)
			{
				_logger.Warn("Unable to shutdown channels", ex);
			}
		}

		private void OnRecoverySucceeded(object sender, EventArgs eventArgs)
		{
			lock (_locker)
			{
				_isConnected = true;
			}
			_logger.DebugFormat(LogMessages.ConnectionRecover, _host);
			var events = ConnectionRecovered;
			try
			{
				events?.Invoke(sender, eventArgs);
			}
			catch (Exception ex)
			{
				_logger.Warn(LogMessages.ConnectionRecoverWarn, ex);
			}
		}
#endregion
	}
}