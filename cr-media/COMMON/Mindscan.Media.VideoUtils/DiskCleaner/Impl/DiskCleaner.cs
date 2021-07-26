using System;
using System.Threading;
using Mindscan.Media.Utils.IoC.Impl;
using Mindscan.Media.Utils.Scheduler;
using Mindscan.Media.VideoUtils.Config;

namespace Mindscan.Media.VideoUtils.DiskCleaner.Impl
{
	internal class DiskCleaner : IDiskCleaner
	{
		private readonly IDiskCleanerConfig _config;
		private readonly ITaskSchedulerFactory _taskSchedulerFactory;
		private ITaskScheduler _scheduler;

		public DiskCleaner(
			IVideoUtilsConfig config,
			ITaskSchedulerFactory taskSchedulerFactory)
		{
			_config = config.DiskCleanerConfig;
			_taskSchedulerFactory = taskSchedulerFactory;
		}

		public void Start(CancellationToken token)
		{
			if (_config.Debug)
				return;

			_scheduler = _taskSchedulerFactory.GetScheduler(token);
			var scheduledTask = Dependency.Resolver.Resolve<IScheduledTask>("DiscCleaner");

			_scheduler
				.Schedule(scheduledTask, config =>
				{
					config.ConcurrentExecution = false;
					config.RepeatInterval = TimeSpan.FromHours(_config.HoursOfDeleteDelay);
				});
		}
	}
}