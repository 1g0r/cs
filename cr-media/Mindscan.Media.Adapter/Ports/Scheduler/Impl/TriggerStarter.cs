using System.Threading;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Messages;
using Mindscan.Media.Messages.Scheduler;
using Mindscan.Media.UseCase.Ports;
using Mindscan.Media.Utils.Broker;

namespace Mindscan.Media.Adapter.Ports.Scheduler.Impl
{
	internal sealed class TriggerStarter : ITriggerStarter
	{
		private readonly IMessageBrokerBuilder _brokerBuilder;
		public TriggerStarter(IMessageBrokerBuilder brokerBuilder)
		{
			_brokerBuilder = brokerBuilder;
		}
		public void Fire(Trigger trigger, CancellationToken token)
		{
			if (token.IsCancellationRequested)
				return;
			var message = new RunScheduledJobMessage
			{
				Id = trigger.Id,
				FeedId = trigger.FeedId,
				RoutingKey = trigger.RoutingKey,
				VirtualHost = trigger.VirtualHost,
				Enabled = trigger.Enabled,
				RepeatInterval = trigger.RepeatInterval,
				CreatedAtUtc = trigger.CreatedAtUtc,
				FireTimeUtc = trigger.FireTimeUtc,
				FireCount = trigger.FireCount,
				Payload = trigger.Payload,
				UpdatedAtUtc = trigger.UpdatedAtUtc
			};
			var broker = _brokerBuilder.Build(token, trigger.VirtualHost ?? "Scheduler");
			broker.Send(CommonExchanges.Scheduler, trigger.RoutingKey, message);
		}
	}
}