using System.Threading;

namespace Mindscan.Media.Utils.Scheduler
{
	public interface IScheduledTask
	{
		void Execute(CancellationToken token);
	}
}