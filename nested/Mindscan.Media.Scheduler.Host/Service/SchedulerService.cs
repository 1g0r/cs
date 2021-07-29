using System;
using System.Threading;
using Mindscan.Media.Adapter.Install;
using Mindscan.Media.Messages;
using Mindscan.Media.Scheduler.Host.Consumer;
using Mindscan.Media.Utils.Broker;
using Mindscan.Media.Utils.Host;
using Mindscan.Media.Utils.Install;
using Mindscan.Media.Utils.IoC.Impl;
using Mindscan.Media.Utils.IoC;
using Mindscan.Media.Utils.Logger;
using Mindscan.Media.Utils.Scheduler;

namespace Mindscan.Media.Scheduler.Host.Service
{
	internal class SchedulerService : IMediaService
	{
		private readonly ILogger _logger;
		private ITaskScheduler _scheduler;

		public SchedulerService(ILoggerFactory factory)
		{
			_logger = factory.CreateLogger(GetType().Name);
		}

		public void Run(CancellationToken token)
		{
			_logger.Info($"Configure {GetType().Name} service.");

			ConfigureService();

			var factory = Dependency.Resolver.Resolve<ITaskSchedulerFactory>();
			var scheduledTask = Dependency.Resolver.Resolve<IScheduledTask>();

			_scheduler = factory.GetScheduler(token);
			_scheduler
				.Schedule(scheduledTask, config =>
				{
					config.ConcurrentExecution = false;
					config.RepeatInterval = new TimeSpan(0, 0, 10);
				});

			Dependency.Resolver.Resolve<IMessageBrokerBuilder>()
				.Build(token, "scheduler")
				.Subscribe<AddTriggerMessageConsumer>(
					$"{CommonExchanges.Scheduler}.AddTrigger",
					CommonExchanges.Scheduler,
					"AddTrigger", 1);

			_logger.Info($"{GetType().Name} service is configured.");
		}

		public void Stop(CancellationToken token)
		{
			_scheduler.Dispose();
			Dependency.Instance.Dispose();
			_logger.Info($"Stop {GetType().Name} service.");
		}

		private void ConfigureService()
		{
			Dependency.Registrar.InstallUtils("utils")
				.UseLogger()
				.UseBroker()
				.UseDbConnector()
				.UseScheduler();

			Dependency.Registrar.InstallUseCases("repository");

			Dependency.Registrar
				.Register<IScheduledTask, HartbeatTask>(Lifetime.Singleton);

		}
	}
}