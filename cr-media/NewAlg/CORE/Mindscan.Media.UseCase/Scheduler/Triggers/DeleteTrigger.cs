using System;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Helpers;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scheduler.Triggers
{
	public class DeleteTrigger : TriggersUseCaseBase
	{
		public DeleteTrigger(ITriggersRepository repository)
			:base(repository)
		{
		}

		public int Delete(Trigger trigger)
		{
			if (trigger == null)
				throw new ArgumentNullException(nameof(trigger));

			var old = Repository.Find(trigger.Id);
			if(old == null)
				throw new EntityNotFoundException(nameof(Trigger), trigger.Id);
			if(old.UpdatedAtUtc != trigger.UpdatedAtUtc)
				throw new EntityOutdatedException(nameof(Trigger), trigger.Id);

			return Repository.Delete(trigger)
				.ThrowIfZero(() => new EntityFailToDeleteException(nameof(Trigger), trigger.Id));
		}
	}
}