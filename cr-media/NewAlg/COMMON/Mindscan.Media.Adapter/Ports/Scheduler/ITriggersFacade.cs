using System;
using System.Collections.Generic;
using System.Threading;
using Mindscan.Media.Domain.Entities.Scheduler;

namespace Mindscan.Media.Adapter.Ports.Scheduler
{
	public interface ITriggersFacade
	{
		Trigger Add(Trigger trigger);
		IEnumerable<Trigger> Add(long sourceId, Trigger trigger);
		IEnumerable<Trigger> FindTriggersToFire();
		IEnumerable<Tuple<Trigger, Feed, Source>> Find(TriggerFilter filter);
		Trigger Update(Trigger trigger);
		int Delete(Trigger trigger);
		void Fire(Trigger trigger);
		void FireNoCheck(Trigger trigger, CancellationToken token);
	}
}