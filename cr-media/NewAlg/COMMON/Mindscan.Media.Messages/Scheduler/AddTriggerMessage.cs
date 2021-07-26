using System;

namespace Mindscan.Media.Messages.Scheduler
{
	public class AddTriggerMessage
	{
		public long FeedId { get; set; }
		public string RoutingKey { get; set; }
		public string VirtualHost { get; set; }
		public bool Enabled { get; set; }
		public TimeSpan RepeatInterval { get; set; }
		public string Payload { get; set; }
		public DateTime? StartAtUtc { get; set; }
	}
}
