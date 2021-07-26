using System.Threading;

namespace Mindscan.Media.Utils.Scheduler
{
	public interface ITaskSchedulerFactory
	{
		ITaskScheduler GetScheduler(CancellationToken token);
	}
}