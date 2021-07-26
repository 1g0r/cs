using System;
using System.Collections.Generic;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.UseCase.Ports;
using Mindscan.Media.UseCase.Scheduler.Feeds;

namespace Mindscan.Media.Adapter.Ports.Scheduler.Impl
{
	internal class FeedsFacade : IFeedsFacade
	{
		private readonly Lazy<FindFeed> _findFeed;
		private readonly Lazy<AddFeed> _addFeed;
		private readonly Lazy<UpdateFeed> _updateFeed;
		private readonly Lazy<DeleteFeed> _deleteFeed;
		public FeedsFacade(IFeedsRepository feedsRepository, ISourcesRepository sourcesRepository)
		{
			_findFeed = new Lazy<FindFeed>(() => new FindFeed(feedsRepository));
			_addFeed = new Lazy<AddFeed>(() => new AddFeed(feedsRepository, sourcesRepository));
			_updateFeed = new Lazy<UpdateFeed>(() => new UpdateFeed(feedsRepository));
			_deleteFeed = new Lazy<DeleteFeed>(() => new DeleteFeed(feedsRepository));
		}
		public IEnumerable<Feed> Find(FeedFilter filter)
		{
			return _findFeed.Value.Find(filter);
		}

		public Feed Find(long id)
		{
			return _findFeed.Value.Find(id);
		}

		public Feed Add(long sourceId, Feed feed)
		{
			return _addFeed.Value.Add(sourceId, feed);
		}

		public Feed Update(Feed feed)
		{
			return _updateFeed.Value.Update(feed);
		}

		public Feed Update(Feed feed, NormalizedUrl redirectedUrl)
		{
			return _updateFeed.Value.Update(feed, redirectedUrl); 
		}

		public int Delete(Feed feed)
		{
			return _deleteFeed.Value.Delete(feed);
		}
	}
}