using System;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Helpers;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scheduler.Feeds
{
	public class DeleteFeed : FeedUseCaseBase
	{
		public DeleteFeed(IFeedsRepository feedsRepository):base(feedsRepository)
		{
			
		}

		public int Delete(Feed feed)
		{
			if(feed == null)
				throw new ArgumentNullException(nameof(feed));

			CheckDateOfLastUpdate(feed);

			return FeedsRepository.Delete(feed)
				.ThrowIfZero(() => new EntityFailToDeleteException(nameof(Feed), feed.Id));
		}
	}
}