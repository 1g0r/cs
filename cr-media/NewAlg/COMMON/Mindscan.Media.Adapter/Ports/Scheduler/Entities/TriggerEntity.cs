using System;
using Mindscan.Media.Domain.Entities.Scheduler;

namespace Mindscan.Media.Adapter.Ports.Scheduler.Entities
{
	internal class TriggerEntity : EntityBase
	{
		public string RoutingKey { get; set; }
		public string VirtualHost { get; set; }
		public long FeedId { get; set; }
		public bool Enabled { get; set; }
		public TimeSpan RepeatInterval { get; set; }
		public DateTime? StartAtUtc { get; set; }
		public DateTime FireTimeUtc { get; set; }
		public string Payload { get; set; }
		public int FireCount { get; set; }

		internal static TriggerEntity ToEntity(Trigger trigger)
		{
			if (trigger == null)
				return null;

			return new TriggerEntity
			{
				Id = trigger.Id,
				FeedId = trigger.FeedId,
				RoutingKey = trigger.RoutingKey,
				VirtualHost = trigger.VirtualHost,
				Enabled = trigger.Enabled,
				StartAtUtc = trigger.StartAtUtc,
				CreatedAtUtc = trigger.CreatedAtUtc,
				RepeatInterval = trigger.RepeatInterval,
				Payload = trigger.Payload,
				FireCount = trigger.FireCount,
				FireTimeUtc = trigger.FireTimeUtc,
				UpdatedAtUtc = trigger.UpdatedAtUtc
			};
		}


		internal Trigger FromEntity()
		{
			return Trigger.GetBuilder()
				.Id(Id)
				.FeedId(FeedId)
				.RoutingKey(RoutingKey)
				.VirtualHost(VirtualHost)
				.Enabled(Enabled)
				.StartAtUtc(StartAtUtc)
				.RepeatInterval(RepeatInterval)
				.Payload(Payload)
				.CreatedAtUtc(CreatedAtUtc)
				.UpdatedAtUtc(UpdatedAtUtc)
				.FireCount(FireCount)
				.FireTimeUtc(FireTimeUtc)
				.Build();
		}
	}
}