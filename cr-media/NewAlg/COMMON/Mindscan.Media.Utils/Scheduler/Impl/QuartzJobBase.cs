using System;
using System.Threading.Tasks;
using Mindscan.Media.Utils.Logger;
using Quartz;

namespace Mindscan.Media.Utils.Scheduler.Impl
{
	internal abstract class QuartzJobBase : IJob
	{
		private readonly ILogger _logger;

		protected QuartzJobBase()
		{
			_logger = LoggerManager.CreateLogger(GetType().Name);
		}

		public async Task Execute(IJobExecutionContext context)
		{
			try
			{
				var task = context.JobDetail.JobDataMap[nameof(IScheduledTask)] as IScheduledTask;
				if (task != null)
				{
					await Task.Run(() => task.Execute(context.CancellationToken));
				}
			}
			catch (Exception ex)
			{
				_logger.Error("Unable to do the job.", ex);
			}
		}
	}

	internal sealed class ConcurrentQuartzJob : QuartzJobBase
	{

	}

	[DisallowConcurrentExecution]
	internal sealed class NotConcurrentQuartzJob : QuartzJobBase
	{

	}
}