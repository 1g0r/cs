using System;

namespace Mindscan.Media.Messages.Scheduler
{
	public class RunScheduledJobMessage
	{
		public long Id { get; set; }
		public long FeedId { get; set; }
		public string RoutingKey { get; set; }
		public string VirtualHost { get; set; }
		public bool Enabled { get; set; }
		public TimeSpan RepeatInterval { get; set; }
		public DateTime CreatedAtUtc { get; set; }
		public DateTime FireTimeUtc { get; set; }
		public string Payload { get; set; }
		public int FireCount { get; set; }
		public DateTime UpdatedAtUtc { get; set; }
	}
}