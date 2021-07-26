using System;
using System.Threading;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scheduler.Triggers
{
	public class FireTrigger : TriggersUseCaseBase
	{
		private readonly ITriggerStarter _starter;
		public FireTrigger(ITriggersRepository repository, ITriggerStarter starter) 
			: base(repository)
		{
			_starter = starter;
		}

		public void FireNoCheck(Trigger trigger, CancellationToken token)
		{
			if(trigger == null)
				throw new ArgumentNullException(nameof(trigger));
			
			_starter.Fire(trigger, token);
		}

		public void Fire(Trigger trigger)
		{
			if (trigger == null)
				throw new ArgumentNullException(nameof(trigger));
			var old = Repository.Find(trigger.Id);
			if (old == null)
				throw new EntityNotFoundException(nameof(Trigger), trigger.Id);
			if (old.UpdatedAtUtc != trigger.UpdatedAtUtc)
				throw new EntityOutdatedException(nameof(Trigger), trigger.Id);

			_starter.Fire(trigger, CancellationToken.None);
		}
	}
}