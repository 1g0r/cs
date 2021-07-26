using System;
using Mindscan.Media.Utils.Broker.Impl;
using Mindscan.Media.Utils.Logger;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace Mindscan.Media.Utils.Broker.Consumer
{
	public abstract class MediaConsumerBase
	{
		internal abstract void ProcessMessage(BasicDeliverEventArgs args, ConsumerContext context, BrokerConnection connection, ConsumerChannel channel);
	}

	public abstract class MediaConsumerBase<TMessage> : MediaConsumerBase
	{
		protected readonly ILogger Logger;
		protected MediaConsumerBase(ILoggerFactory loggerFactory)
		{
			Logger = loggerFactory.CreateLogger(GetType().Name);
		}
		internal override void ProcessMessage(BasicDeliverEventArgs args, ConsumerContext context, BrokerConnection connection, 
			ConsumerChannel channel)
		{
			if (context.Token.IsCancellationRequested)
				return;
			try
			{
				Consume(new ConsumerContext<TMessage>(context), connection);
			}
			catch (JsonReaderException jre)
			{
				Logger.Error(LogMessages.ConsumeDeserializeError, jre);
				connection.TrySendError(context.Queue, args, jre);
			}
			catch (Exception ex)
			{
				Logger.Error(LogMessages.ConsumeError, ex);
				connection.TrySendError(context.Queue, args, ex);
			}
			finally
			{
				channel.Ack(args.DeliveryTag);
			}
		}

		protected abstract void Consume(ConsumerContext<TMessage> context, IMessageBroker broker);
	}
}