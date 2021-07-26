using System;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Helpers;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scheduler.Feeds
{
	public class AddFeed : FeedUseCaseBase
	{
		private readonly ISourcesRepository _sourcesRepository;

		public AddFeed(IFeedsRepository feedsRepository, ISourcesRepository sourcesRepository)
			:base(feedsRepository)
		{
			_sourcesRepository = sourcesRepository;
		}

		public Feed Add(long sourceId, Feed feed)
		{
			if (feed == null)
				throw new ArgumentNullException(nameof(feed));

			var source = _sourcesRepository.Find(sourceId);
			if (source == null)
			{
				throw new EntityNotFoundException("Source", sourceId);
			}

			return FeedsRepository.Add(sourceId, feed)
				.ThrowIfNull(() => new EntityFailToAddException(nameof(Feed)));
		}
	}
}
