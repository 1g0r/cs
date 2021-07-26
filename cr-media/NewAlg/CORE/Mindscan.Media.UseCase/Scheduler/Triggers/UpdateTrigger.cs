using System;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Helpers;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scheduler.Triggers
{
	public class UpdateTrigger : TriggersUseCaseBase
	{
		public UpdateTrigger(ITriggersRepository repository)
			:base(repository)
		{
			
		}

		public Trigger Update(Trigger trigger)
		{
			if (trigger == null)
				throw new ArgumentNullException(nameof(trigger));

			var old = Repository.Find(trigger.Id);
			if (old == null)
				throw new EntityNotFoundException(nameof(Trigger), trigger.Id);
			if (old.UpdatedAtUtc != trigger.UpdatedAtUtc)
				throw new EntityOutdatedException(nameof(Trigger), trigger.Id);
			if (HasNoChanges(old, trigger))
				throw new EntityHasNoChangesException(nameof(Trigger), trigger.Id);

			if (trigger.StartAtUtc.HasValue && old.StartAtUtc != trigger.StartAtUtc)
			{
				SetStartAt(trigger);
			}

			return Repository.Update(trigger)
				.ThrowIfNull(() => new EntityNotFoundException(nameof(Trigger), trigger.Id));
		}

		private static bool HasNoChanges(Trigger old, Trigger @new)
		{
			if (old.Id != @new.Id)
				return true;

			return 
				old.VirtualHost == @new.VirtualHost &&
				old.RoutingKey == @new.RoutingKey &&
				old.Enabled == @new.Enabled &&
				old.RepeatInterval == @new.RepeatInterval &&
				old.Payload == @new.Payload &&
				old.StartAtUtc == @new.StartAtUtc;
		}
	}
}