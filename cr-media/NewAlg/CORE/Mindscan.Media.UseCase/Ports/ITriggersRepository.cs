using System;
using System.Collections.Generic;
using Mindscan.Media.Domain.Entities.Scheduler;

namespace Mindscan.Media.UseCase.Ports
{
	public interface ITriggersRepository
	{
		bool Exists(Trigger trigger);
		Trigger Add(Trigger trigger);
		IEnumerable<Trigger> Add(Source source, Trigger trigger);
		Trigger Find(long id);
		IEnumerable<Tuple<Trigger, Feed, Source>> Find(TriggerFilter filter);
		IEnumerable<Trigger> FindTriggersToFire();
		Trigger Update(Trigger trigger);
		int Delete(Trigger trigger);
	}
}
