namespace Mindscan.Media.Utils.Broker.Impl
{
	internal static class LogMessages
	{
		public const string DeclareQueue = "DECLARE.queue '{0}'.";
		public const string DeclareExchange = "DECLARE.exchange '{0}' of type '{1}' and durable = {2}.";

		public const string BoundQueue = "BOUND.queue '{0}' to the exchange '{1}' with routing key '{2}'.";

		public const string SendMessageNoRoutingKey = "SEND.message to exchange:'{0}'.";
		public const string SendMessage = "SEND.message to exchange:'{0}' with routingKey: '{1}'.";
		public const string SendAck = "SEND.ack";

		public const string ConsumeMessage = "CONSUME.message '{0}' from the queue '{1}'.";
		public const string ConsumeWarn = "CONSUME.warn unable to resolve consumer for the queue '{0}'";
		public const string ConsumeError = "CONSUME.error.";
		public const string ConsumeDeserializeError = "CONSUME.deserialize.error unable to deserealize message";
		public const string ConsumeCancelled = "CONSUME.canselled";
		public const string ConsumeRecover = "CONSUME.recover of the queue '{0}' successfull!";

		public const string SubscribeConsumer = "SUBSCRIBE.consumer of message type '{0}' to queue '{1}'.";

		public const string ConnectionEstablished = "CONNECTION.established to host '{0}'.";
		public const string ConnectionShutdown = "CONNECTION.shutdown to host '{0}'.";
		public const string ConnectionShutdownWarn = "CONNECTION.shutdown.warn unable to shutdown channels.";
		public const string ConnectionRecover = "CONNECTION.recover to {0}.";
		public const string ConnectionRecoverWarn = "CONNECTION.recover.warn unable to recover channels.";
	}
}