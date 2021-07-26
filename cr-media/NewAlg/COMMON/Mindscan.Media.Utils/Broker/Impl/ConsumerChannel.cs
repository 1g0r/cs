using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mindscan.Media.Utils.Broker.Consumer;
using Mindscan.Media.Utils.Broker.Temp;
using Mindscan.Media.Utils.IoC.Impl;
using Mindscan.Media.Utils.IoC;
using Mindscan.Media.Utils.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mindscan.Media.Utils.Broker.Impl
{
	internal class ConsumerChannel : ChannelBase
	{
		private readonly string _queue;
		private ushort _prefetchCount;
		private EventingBasicConsumer _consumer;
		private volatile bool _reconfigure;

		public ConsumerChannel(string queue, string exchange, string routingKey, BrokerConnection connection, CancellationToken token, ILoggerFactory factory)
			: base(exchange, routingKey, connection, token, factory)
		{
			_reconfigure = false;
			_queue = queue;
		}

		public void Subscribe<TMessage, TConsumer>(ushort prefetchCount) where TConsumer : MediaConsumerBase<TMessage>
		{
			_prefetchCount = prefetchCount;
			Dependency.Registrar.Register<MediaConsumerBase, TConsumer>(typeof(TMessage).FullName, Lifetime.Singleton);
			if (_consumer == null)
			{
				lock (this)
				{
					if (_consumer == null)
					{
						CreateConsumer();
					}
				}
			}
			Logger.DebugFormat(LogMessages.SubscribeConsumer, typeof(TMessage), _queue);
		}

		public void Subscribe<TConsumer>(ushort prefetchCount) where TConsumer : MediaConsumerBase
		{
			_prefetchCount = prefetchCount;
			Dependency.Registrar.Register<MediaConsumerBase, TConsumer>(_queue, Lifetime.Singleton);
			if (_consumer == null)
			{
				lock (this)
				{
					if (_consumer == null)
					{
						CreateConsumer();
					}
				}
			}
			Logger.DebugFormat(LogMessages.SubscribeConsumer, "general type", _queue);
		}

		public void Ack(ulong deliveryTag)
		{
			if (_reconfigure)
				return;
			Call(m =>
			{
				Logger.Debug(LogMessages.SendAck);
				m.BasicAck(deliveryTag, false);
			});
		}

		public override void Dispose()
		{
			if (_consumer != null)
			{
				_consumer.ConsumerCancelled -= OnConsumerCanceled;
			}
			base.Dispose();
		}

		protected override void CustomConfigure()
		{
			CreateQueue(_queue, _prefetchCount);
		}

		protected new void Reconfigure()
		{
			if (_reconfigure)
			{
				lock (this)
				{
					if (_reconfigure)
					{
						base.Reconfigure();
						CreateConsumer();
						_reconfigure = false;
					}
				}
				Logger.Debug(string.Format(LogMessages.ConsumeRecover, _queue));
			}
		}

		private void CreateConsumer()
		{
			_consumer = Call(m => new EventingBasicConsumer(m));
			if (_consumer != null)
			{
				_consumer.Received += OnReceived;
				_consumer.ConsumerCancelled += OnConsumerCanceled;

				Call(m => m.BasicConsume(_queue, false, _consumer));
			}
		}

		private bool TryResolveHandler(BasicDeliverEventArgs args, out MediaConsumerBase handler, out MassTransitEnvelop envelop)
		{
			//Try resolve by message type
			Exception error;
			envelop = null;
			var name = GetMessageTypeName(args);
			if (name != null && TryResolveHandler(name, out handler, out error))
			{
				return true;
			}
			// Try resolve by queue name
			if (TryResolveHandler(_queue, out handler, out error))
			{
				return true;
			}
			//Try resolve mass transit message
			name = TryResolveMassTransitMessage(args, out envelop);
			if (name != null && TryResolveHandler(name, out handler, out error))
			{
				return true;
			}
			// Nope nothing here
			Logger.Warn(string.Format(LogMessages.ConsumeWarn, name), error);
			return false;
		}

		private bool TryResolveHandler(string name, out MediaConsumerBase handler, out Exception error)
		{
			error = null;
			try
			{
				handler = Dependency.Resolver.Resolve<MediaConsumerBase>(name);
				return true;
			}
			catch (Exception ex)
			{
				handler = null;
				error = ex;
				return false;
			}
		}

		private string TryResolveMassTransitMessage(BasicDeliverEventArgs args, out MassTransitEnvelop envelop)
		{
			try
			{
				envelop = JsonConvert.DeserializeObject<MassTransitEnvelop<JObject>>(Encoding.UTF8.GetString(args.Body));
				return envelop.MessageType[0].Replace("urn:message:", "").Replace(":", ".");
			}
			catch (Exception ex)
			{
				Logger.Warn("Unable to resolve mass transit message");
				envelop = null;
				return null;
			}
		}

		private string GetMessageTypeName(BasicDeliverEventArgs args)
		{
			if (args?.BasicProperties?.Headers?.ContainsKey(MessageBrokerHeaders.Type) ?? false)
			{
				var data = args.BasicProperties.Headers[MessageBrokerHeaders.Type] as byte[];
				if (data != null)
				{
					return Encoding.UTF8.GetString(data);
				}
			}

			return null;
		}

		#region event handlers
		public override void OnConnectionClosed(object sender, ShutdownEventArgs shutdownEventArgs)
		{
			Logger.Debug("Consumer CONNECTION closed!");
			base.OnConnectionClosed(sender, shutdownEventArgs);
			lock (this)
			{
				_reconfigure = true;
				if (_consumer != null)
				{
					_consumer.Received -= OnReceived;
					_consumer.ConsumerCancelled -= OnConsumerCanceled;
				}
			}
		}

		public override void OnConnectionRecovered(object sender, EventArgs eventArgs)
		{
			Logger.Debug("Consumer CONNECTION recovered!");
			Reconfigure();
		}

		protected override void OnModelShutdown(object sender, ShutdownEventArgs shutdownEventArgs)
		{
			Logger.Debug("Consumer MODEL shutdown!");
			base.OnModelShutdown(sender, shutdownEventArgs);
			lock (this)
			{
				_reconfigure = true;
				if (_consumer != null)
				{
					_consumer.Received -= OnReceived;
					_consumer.ConsumerCancelled -= OnConsumerCanceled;
				}
			}
			// Восстановим очередь
			Reconfigure();
		}

		private void OnReceived(object sender, BasicDeliverEventArgs args)
		{
			if (_reconfigure || !Connection.IsAlive)
				return;

			MediaConsumerBase handler;
			MassTransitEnvelop envelop;
			if (TryResolveHandler(args, out handler, out envelop))
			{
				if (Logger.IsDebugEnabled)
				{
					Logger.DebugFormat(LogMessages.ConsumeMessage, Encoding.UTF8.GetString(args.Body), _queue);
				}
				if (!_reconfigure)
				{
					try
					{
						handler.ProcessMessage(
							args,
							new ConsumerContext(args, _queue, envelop, Token),
							Connection,
							this);
					}
					catch (Exception ex)
					{
						Logger.Error(LogMessages.ConsumeError, ex);
					}
				}
			}
			else
			{
				Call(m =>
				{
					Logger.Debug(LogMessages.SendAck);
					m.BasicAck(args.DeliveryTag, false);
				});
			}
		}

		private void OnConsumerCanceled(object sender, ConsumerEventArgs consumerEventArgs)
		{
			//Происходит при удалении очереди руками
			Logger.Debug(LogMessages.ConsumeCancelled);
			lock (this)
			{
				_reconfigure = true;
				_consumer.ConsumerCancelled -= OnConsumerCanceled;
				_consumer.Received -= OnReceived;
			}
			// Восстановим очередь
			Reconfigure();
		}
		#endregion

	}
}