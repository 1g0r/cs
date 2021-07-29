using System.Threading;
using Mindscan.Media.Adapter.Install;
using Mindscan.Media.Utils.Broker;
using Mindscan.Media.Utils.Logger;
using Mindscan.Media.Utils.Scheduler;

namespace Mindscan.Media.Scheduler.Host.Service
{
	internal sealed class HartbeatTask : IScheduledTask
	{
		private readonly ILogger _logger;
		private readonly IUseCases _useCases;
		private readonly IMessageBrokerBuilder _brokerBuilder;
		public HartbeatTask(ILoggerFactory loggerFactory, IUseCases useCases, IMessageBrokerBuilder brokerBuilder)
		{
			_logger = loggerFactory.CreateLogger(GetType().Name);
			_useCases = useCases;
			_brokerBuilder = brokerBuilder;
		}
		public void Execute(CancellationToken token)
		{
			var count = 0;

			foreach (var trigger in _useCases.Triggers.FindTriggersToFire())
			{
				_useCases.Triggers.FireNoCheck(trigger, token);
				count++;
			}
			if (count > 0)
			{
				_logger.Debug($"Messages were scheduled {count}.");
			}
			else
			{
				_logger.Debug("No messages were scheduled.");
			}
		}
	}
}