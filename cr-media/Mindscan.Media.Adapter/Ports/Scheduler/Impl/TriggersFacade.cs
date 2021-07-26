using System;
using System.Collections.Generic;
using System.Threading;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.UseCase.Ports;
using Mindscan.Media.UseCase.Scheduler.Triggers;

namespace Mindscan.Media.Adapter.Ports.Scheduler.Impl
{
	internal class TriggersFacade : ITriggersFacade
	{
		private readonly Lazy<AddTrigger> _add;
		private readonly Lazy<FindTrigger> _find;
		private readonly Lazy<DeleteTrigger> _delete;
		private readonly Lazy<UpdateTrigger> _update;
		private readonly Lazy<FireTrigger> _fire;
		public TriggersFacade(
			ITriggersRepository triggersRepository, 
			IFeedsRepository feedsRepository, 
			ISourcesRepository sourcesRepository, 
			ITriggerStarter starter)
		{
			_add = new Lazy<AddTrigger>(() => new AddTrigger(triggersRepository, feedsRepository, sourcesRepository));
			_find = new Lazy<FindTrigger>(() => new FindTrigger(triggersRepository));
			_delete = new Lazy<DeleteTrigger>(() => new DeleteTrigger(triggersRepository));
			_update = new Lazy<UpdateTrigger>(() => new UpdateTrigger(triggersRepository));
			_fire = new Lazy<FireTrigger>(() => new FireTrigger(triggersRepository, starter));
		}
		public Trigger Add(Trigger trigger)
		{
			return _add.Value.Add(trigger);
		}

		public IEnumerable<Trigger> Add(long sourceId, Trigger trigger)
		{
			return _add.Value.Add(sourceId, trigger);
		}

		public IEnumerable<Trigger> FindTriggersToFire()
		{
			return _find.Value.FindTriggersToFire();
		}

		public IEnumerable<Tuple<Trigger, Feed, Source>> Find(TriggerFilter filter)
		{
			return _find.Value.Find(filter);
		}

		public Trigger Update(Trigger trigger)
		{
			return _update.Value.Update(trigger);
		}

		public int Delete(Trigger trigger)
		{
			return _delete.Value.Delete(trigger);
		}

		public void Fire(Trigger trigger)
		{
			_fire.Value.Fire(trigger);
		}

		public void FireNoCheck(Trigger trigger, CancellationToken token)
		{
			_fire.Value.FireNoCheck(trigger, token);
		}
	}
}