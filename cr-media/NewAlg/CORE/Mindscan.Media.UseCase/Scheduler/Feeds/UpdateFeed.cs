using System;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Helpers;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Scheduler.Feeds
{
	public class UpdateFeed : FeedUseCaseBase
	{
		public UpdateFeed(IFeedsRepository feedsRepository):base(feedsRepository)
		{
		}

		public Feed Update(Feed feed)
		{
			if (feed == null)
				throw new ArgumentNullException(nameof(feed));

			var old = CheckDateOfLastUpdate(feed);

			if (HasNoChanges(old, feed))
				throw new EntityHasNoChangesException("Feed", feed.Id);

			return FeedsRepository.Update(feed)
				.ThrowIfNull(() => new EntityFailToUpdateException(nameof(Feed), feed.Id));
		}

		public Feed Update(Feed feed, NormalizedUrl actualUrl)
		{
			if(feed == null)
				throw new ArgumentNullException(nameof(feed));
			if(actualUrl == null)
				throw new ArgumentNullException(nameof(actualUrl));

			CheckDateOfLastUpdate(feed);

			var notEqual = CompareNotEqual(actualUrl);
			if (notEqual(feed.OriginalUrl) && notEqual(feed.ActualUrl))
			{
				feed.ActualUrl = actualUrl;
				return FeedsRepository.UpdateActualUrl(feed)
					.ThrowIfNull(() => new EntityFailToUpdateException(nameof(Feed), feed.Id));
			}

			return feed;
		}

		private bool HasNoChanges(Feed old, Feed @new)
		{
			if (old.Id != @new.Id)
				return true;

			return old.OriginalUrl == @new.OriginalUrl &&
				old.ActualUrl == @new.ActualUrl &&
				old.Type == @new.Type &&
				old.Encoding == @new.Encoding &&
				old.Description == @new.Description &&
				old.AdditionalInfo == @new.AdditionalInfo;
		}

		private static Func<NormalizedUrl, bool> CompareNotEqual(NormalizedUrl first)
		{
			return second => first?.Prefix != second?.Prefix || first?.Tail != second?.Tail;
		}
	}
}