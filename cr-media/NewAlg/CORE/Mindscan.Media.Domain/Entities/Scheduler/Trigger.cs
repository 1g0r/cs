using System;

namespace Mindscan.Media.Domain.Entities.Scheduler
{
	public class Trigger : EntityBase
	{
		internal Trigger() { }
		public DateTime FireTimeUtc { get; internal set; }
		public int FireCount { get; internal set; }
		public long FeedId { get; internal set; }
		public string RoutingKey { get; internal set; }
		public string VirtualHost { get; internal set; }
		public bool Enabled { get; internal set; }
		public TimeSpan RepeatInterval { get; internal set; }
		public DateTime? StartAtUtc { get; internal set; }
		public string Payload { get; internal set; }

		public static TriggerBuilder GetBuilder()
		{
			return new TriggerBuilder();
		}
	}
}