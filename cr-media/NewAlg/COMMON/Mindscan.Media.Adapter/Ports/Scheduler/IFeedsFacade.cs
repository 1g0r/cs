using System;
using System.Collections.Generic;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Scheduler;

namespace Mindscan.Media.Adapter.Ports.Scheduler
{
	public interface IFeedsFacade
	{
		IEnumerable<Feed> Find(FeedFilter filter);
		Feed Find(long id);
		Feed Add(long sourceId, Feed feed);
		Feed Update(Feed feed);
		Feed Update(Feed feed, NormalizedUrl redirectedUrl);
		int Delete(Feed feed);
	}
}