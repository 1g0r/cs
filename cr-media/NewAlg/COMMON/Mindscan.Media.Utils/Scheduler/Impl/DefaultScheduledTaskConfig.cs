using System;
using System.Threading;

namespace Mindscan.Media.Utils.Scheduler.Impl
{
	internal sealed class DefaultScheduledTaskConfig : IScheduledTaskConfig
	{
		public DateTimeOffset StartAtUtc { get; set; } = DateTimeOffset.UtcNow;
		public bool ConcurrentExecution { get; set; } = true;
		public TimeSpan RepeatInterval { get; set; } = new TimeSpan(0, 0, 30);
		public CancellationToken Token { get; set; } = CancellationToken.None;
	}
}