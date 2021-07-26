using System;
using System.Collections.Generic;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Helpers;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scheduler.Triggers
{
	public class AddTrigger : TriggersUseCaseBase
	{
		private readonly IFeedsRepository _feedsRepository;
		private readonly ISourcesRepository _sourcesRepository;
		public AddTrigger(ITriggersRepository repository, IFeedsRepository feedsRepository, ISourcesRepository sourcesRepository)
			:base(repository)
		{
			_feedsRepository = feedsRepository;
			_sourcesRepository = sourcesRepository;
		}

		public Trigger Add(Trigger trigger)
		{
			if (trigger == null)
				throw new ArgumentNullException(nameof(trigger));

			var feed = _feedsRepository.Find(trigger.FeedId);
			if(feed == null)
				throw new EntityNotFoundException(nameof(Feed), trigger.Id);
			if(Repository.Exists(trigger))
				throw new EntityAlreadyExistsException(nameof(Trigger));

			SetTriggerDates(trigger);

			return Repository.Add(trigger)
				.ThrowIfNull(() => new EntityFailToAddException(nameof(Trigger)));
		}

		public IEnumerable<Trigger> Add(long sourceId, Trigger trigger)
		{
			if (trigger == null)
				throw new ArgumentNullException(nameof(trigger));

			var source = _sourcesRepository.Find(sourceId);
			if(source == null)
				throw new EntityNotFoundException(nameof(Source), sourceId);

			SetTriggerDates(trigger);

			return Repository.Add(source, trigger).EnsureNotNull();
		}

		protected static void SetTriggerDates(Trigger trigger)
		{
			trigger.CreatedAtUtc = trigger.UpdatedAtUtc = DateTime.UtcNow;
			SetStartAt(trigger);
			trigger.FireTimeUtc = trigger.StartAtUtc ?? DateTime.Now;
		}
	}
}