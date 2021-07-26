using System;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scheduler.Triggers
{
	public abstract class TriggersUseCaseBase
	{
		protected readonly ITriggersRepository Repository;

		protected TriggersUseCaseBase(ITriggersRepository repository)
		{
			Repository = repository;
		}
		protected static void SetStartAt(Trigger trigger)
		{
			var now = DateTime.UtcNow;
			if (!trigger.StartAtUtc.HasValue)
			{
				trigger.StartAtUtc = now;
			}
			else
			{
				if (trigger.StartAtUtc >= now)
					return;

				var startAt = now.Date.Add(trigger.StartAtUtc.Value.TimeOfDay);
				if (startAt < now)
				{
					startAt = startAt.AddDays(1);
				}
				trigger.StartAtUtc = startAt;
			}
		}
	}
}