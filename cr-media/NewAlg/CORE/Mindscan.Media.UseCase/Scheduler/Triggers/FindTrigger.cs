using System;
using System.Collections.Generic;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.UseCase.Helpers;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scheduler.Triggers
{
	public class FindTrigger: TriggersUseCaseBase
	{
		public FindTrigger(ITriggersRepository repository)
			:base(repository)
		{
			
		}

		public IEnumerable<Trigger> FindTriggersToFire()
		{
			return Repository.FindTriggersToFire()
				.EnsureNotNull();
		}

		public IEnumerable<Tuple<Trigger, Feed, Source>> Find(TriggerFilter filter)
		{
			if (filter == null)
				throw new ArgumentNullException(nameof(filter));

			filter.SetDefaults();

			return Repository.Find(filter)
				.EnsureNotNull();
		}
	}
}