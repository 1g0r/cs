using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mindscan.Media.Utils.Scheduler
{
	public interface ITaskScheduler : IDisposable
	{
		ITaskScheduler Schedule(IScheduledTask task, Action<IScheduledTaskConfig> configurator = null);
	}
}
