using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scheduler.Feeds
{
	public abstract class FeedUseCaseBase
	{
		protected readonly IFeedsRepository FeedsRepository;

		protected FeedUseCaseBase(IFeedsRepository repository)
		{
			FeedsRepository = repository;
		}

		protected Feed CheckDateOfLastUpdate(Feed feed)
		{
			var old = FeedsRepository.Find(feed.Id);
			if (old == null)
				throw new EntityNotFoundException("Feed", feed.Id);
			if (old.UpdatedAtUtc != feed.UpdatedAtUtc)
				throw new EntityOutdatedException("Feed", feed.Id);
			return old;
		}
	}
}