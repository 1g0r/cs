using System;
using System.Threading;
using Mindscan.Media.Utils.Logger;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;

namespace Mindscan.Media.Utils.Scheduler.Impl
{
	internal sealed class TaskScheduler : ITaskScheduler
	{
		private readonly ILogger _logger;
		private readonly IScheduler _scheduler;
		private readonly CancellationToken _token;
		public TaskScheduler(ILoggerFactory loggerFactory, CancellationToken token)
		{
			_logger = loggerFactory.CreateLogger(GetType().Name);
			_token = token;
			LogProvider.SetCurrentLogProvider(new QuartzLogProvider());
			_scheduler = new StdSchedulerFactory().GetScheduler(_token).Result;
			_scheduler.Start(_token);
		}
		public ITaskScheduler Schedule(IScheduledTask task, Action<IScheduledTaskConfig> configurator)
		{
			var config = new DefaultScheduledTaskConfig();
			configurator?.Invoke(config);

			try
			{
				// define the job and tie it to our HelloJob class
				var type = task.GetType();
				IJobDetail jobDetail;
				if (config.ConcurrentExecution)
				{
					jobDetail = JobBuilder.Create<ConcurrentQuartzJob>()
						.WithIdentity(type.Name, type.Namespace)
						.Build();
				}
				else
				{
					jobDetail = JobBuilder.Create<NotConcurrentQuartzJob>()
						.WithIdentity(type.Name, type.Namespace)
						.Build();
				}

				jobDetail.JobDataMap.Add(nameof(IScheduledTask), task);

				// Trigger the job to run now, and then repeat every 10 seconds
				ITrigger trigger = TriggerBuilder.Create()
					.WithIdentity(type.Name, type.Namespace)
					.StartAt(config.StartAtUtc)
					.WithSimpleSchedule(x => x
						.WithInterval(config.RepeatInterval)
						.RepeatForever())
					.Build();

				// Tell quartz to schedule the job using our trigger
				_scheduler.ScheduleJob(jobDetail, trigger, _token);
			}
			catch (SchedulerException se)
			{
				_logger.Error($"Unable to start IScheduledTask {task.GetType()}", se);
			}
			return this;
		}

		public void Dispose()
		{
			_scheduler?.Shutdown(_token);
		}
	}
}