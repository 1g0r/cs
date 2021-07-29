using Mindscan.Media.Adapter.Install;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Messages.Scheduler;
using Mindscan.Media.Utils.Broker;
using Mindscan.Media.Utils.Broker.Consumer;
using Mindscan.Media.Utils.Logger;

namespace Mindscan.Media.Scheduler.Host.Consumer
{
	class AddTriggerMessageConsumer: MediaConsumerBase<AddTriggerMessage>
	{
		private readonly IUseCases _useCases;
		public AddTriggerMessageConsumer(ILoggerFactory loggerFactory, IUseCases useCases) : base(loggerFactory)
		{
			_useCases = useCases;
		}

		protected override void Consume(ConsumerContext<AddTriggerMessage> context, IMessageBroker broker)
		{
			var trigger = Trigger.GetBuilder()
				.FeedId(context.Message.FeedId)
				.Enabled(context.Message.Enabled)
				.RoutingKey(context.Message.RoutingKey)
				.VirtualHost(context.Message.VirtualHost)
				.RepeatInterval(context.Message.RepeatInterval)
				.Payload(context.Message.Payload)
				.StartAtUtc(context.Message.StartAtUtc)
				.Build();

			_useCases.Triggers.Add(trigger);
		}
	}
}
