using System.Threading;
using Mindscan.Media.Utils.Logger;

namespace Mindscan.Media.Utils.Scheduler.Impl
{
	internal class TaskSchedulerFactory : ITaskSchedulerFactory
	{
		private readonly ILoggerFactory _loggerFactory;
		private static ITaskScheduler _scheduler;
		public TaskSchedulerFactory(ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory;
		}

		public ITaskScheduler GetScheduler(CancellationToken token)
		{
			if (_scheduler == null)
			{
				lock (this)
				{
					if (_scheduler == null)
					{
						_scheduler = new TaskScheduler(_loggerFactory, token);
					}
				}
			}
			return _scheduler;
		}
	}
}