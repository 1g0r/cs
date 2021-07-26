using System;
using System.Collections.Generic;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Scheduler;

namespace Mindscan.Media.UseCase.Ports
{
	public interface IFeedsRepository
	{
		Feed Add(long sourceId, Feed feed);
		Feed Find(long id);
		IEnumerable<Feed> Find(FeedFilter filter);
		int Delete(Feed feed);
		Feed Update(Feed feed);
		Feed UpdateActualUrl(Feed feed);
	}
}
