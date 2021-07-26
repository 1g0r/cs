using System;
using System.Threading;
using Mindscan.Media.Utils.Logger;
using RabbitMQ.Client;

namespace Mindscan.Media.Utils.Broker.Impl
{
	internal abstract class ChannelBase : IDisposable
	{
		private IModel _model;
		protected readonly ILogger Logger;
		protected readonly CancellationToken Token;
		protected readonly string Exchange, RoutingKey;
		private volatile bool _reconfigure;
		protected readonly BrokerConnection Connection;
		
		protected ChannelBase(string exchange, string routingKey, BrokerConnection connection, CancellationToken token, ILoggerFactory factory)
		{
			Logger = factory.CreateLogger(GetType().Name);
			Token = token;
			Exchange = exchange;
			RoutingKey = routingKey ?? string.Empty;
			Connection = connection;
		}

		public virtual void Dispose()
		{
			ReleaseModel();
		}

		// Обращаться к модели только через эти методы (Call), т.к. в спеках указано, что 
		// обращения к модели должны быть однопоточными
		protected void Call(Action<IModel> action)
		{
			Call<object>(model =>
			{
				action(model);
				return null;
			});
		}

		protected T Call<T>(Func<IModel, T> action)
		{
			lock (this)
			{
				if (Connection.IsAlive)
				{
					Configure();
					return action(_model);
				}
				return default(T);
			}
		}

		protected bool CreateExchange(bool writeLog, ushort prefetchCount)
		{
			if (string.IsNullOrWhiteSpace(Exchange))
				return false;

			var exchangeType = !string.IsNullOrWhiteSpace(RoutingKey) ? ExchangeType.Direct : ExchangeType.Fanout;
			prefetchCount = prefetchCount > 0 ? prefetchCount : Connection.Config.PrefetchCount;

			return Call(m =>
			{

				m.ExchangeDeclare(Exchange, exchangeType, true);
				m.BasicQos(0, Connection.Config.Debug ? (ushort)1 : prefetchCount, false);
				if (writeLog)
				{
					Logger.Debug(string.Format(LogMessages.DeclareExchange, Exchange, exchangeType, true));
				}
				return true;
			});
		}

		protected void CreateQueue(string queue, ushort prefetchCount)
		{
			if (CreateQueue(queue) && CreateExchange(true, prefetchCount))
			{
				BindQueue(queue);
			}
		}

		protected abstract void CustomConfigure();

		protected void Reconfigure()
		{
			lock (this)
			{
				ReleaseModel();
				_reconfigure = true;
			}
		}

		private bool CreateQueue(string queue)
		{
			return Call(m =>
			{
				m.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);
				Logger.Debug(string.Format(LogMessages.DeclareQueue, queue));
				return true;
			});
		}

		private void BindQueue(string queue)
		{
			Call(m =>
			{
				m.QueueBind(queue, Exchange, RoutingKey ?? string.Empty);
				Logger.Debug(string.Format(LogMessages.BoundQueue, queue, Exchange, RoutingKey));
			});
		}

		private void Configure()
		{
			try
			{
				if (NeedConfig())
				{
					CreateModel();
					_reconfigure = false;
					CustomConfigure();
				}
			}
			catch (Exception ex)
			{
				Logger.Fatal("Broker configuration failed", ex);
				throw;
			}
		}

		private bool NeedConfig()
		{
			return !Token.IsCancellationRequested && (_model == null || _reconfigure);
		}

		private void CreateModel()
		{
			ReleaseModel();
			_model = Connection.CreateModel();
			if (_model != null)
			{
				_model.ModelShutdown += OnModelShutdown;
			}
		}

		void ReleaseModel()
		{
			if (_model != null)
			{
				_model.ModelShutdown -= OnModelShutdown;
				_model.Dispose();
				_model = null;
			}
			
		}

#region event handlers
		public virtual void OnConnectionClosed(object sender, ShutdownEventArgs shutdownEventArgs)
		{
			Logger.Debug("Connection was closed!");
			Reconfigure();
		}

		public virtual void OnConnectionRecovered(object sender, EventArgs eventArgs)
		{
			
		}

		protected virtual void OnModelShutdown(object sender, ShutdownEventArgs shutdownEventArgs)
		{
			Logger.Debug($"Model was shutdown!");
			Reconfigure();
		}
#endregion
	}
}