using System;
using System.Threading;

namespace Mindscan.Media.Utils.Scheduler
{
	public interface IScheduledTaskConfig
	{
		DateTimeOffset StartAtUtc { get; set; }
		bool ConcurrentExecution { get; set; }
		TimeSpan RepeatInterval { get; set; }
		CancellationToken Token { get; set; }
	}
}