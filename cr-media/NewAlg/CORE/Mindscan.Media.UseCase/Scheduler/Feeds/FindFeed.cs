using System;
using System.Collections.Generic;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Helpers;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scheduler.Feeds
{
	public class FindFeed : FeedUseCaseBase
	{
		public FindFeed(IFeedsRepository repository)
			:base(repository)
		{
		}

		public Feed Find(long id)
		{
			return FeedsRepository.Find(id)
				.ThrowIfNull(() => new EntityNotFoundException(nameof(Feed), id));
		}

		public IEnumerable<Feed> Find(FeedFilter filter)
		{
			if(filter == null)
				throw new ArgumentNullException(nameof(filter));

			filter.SetDefaults();

			return FeedsRepository.Find(filter)
				.EnsureNotNull();
		}
	}
}