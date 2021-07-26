using System;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Enums;

namespace Mindscan.Media.Adapter.Ports.Scheduler.Entities
{
	internal class FeedEntity : EntityBase
	{
		public long SourceId { get; set; }
		public string FeedType { get; set; }
		public string OriginalUrlPrefix { get; set; }
		public string OriginalUrl { get; set; }
		public string ActualUrlPrefix { get; set; }
		public string ActualUrl { get; set; }
		public string Encoding { get; set; }
		public string Description { get; set; }
		public string AdditionalInfo { get; set; }

		public static FeedEntity ToEntity(Feed feed)
		{
			var needActual = NeedActual(feed.OriginalUrl, feed.ActualUrl);
			return new FeedEntity
			{
				Id = feed.Id,
				CreatedAtUtc = feed.CreatedAtUtc,
				UpdatedAtUtc = feed.UpdatedAtUtc,
				FeedType = feed.Type.ToString("G"),
				OriginalUrl = feed.OriginalUrl.Tail,
				OriginalUrlPrefix = feed.OriginalUrl.Prefix,
				ActualUrl = needActual ? feed.ActualUrl.Tail : null,
				ActualUrlPrefix = needActual ? feed.ActualUrl.Prefix : null,
				Encoding = feed.Encoding ?? "utf-8",
				Description = feed.Description,
				AdditionalInfo = feed.AdditionalInfo
			};
		}

		public Feed FromEntity(Source source)
		{
			return Feed.GetBuilder()
				.Id(Id)
				.CreatedAtUtc(CreatedAtUtc)
				.UpdatedAtUtc(UpdatedAtUtc)
				.Encoding(Encoding)
				.OriginalUrl(BuildOriginalUrl())
				.ActualUrl(BuildActualUrl())
				.Source(source)
				.Type((FeedType)Enum.Parse(typeof(FeedType), FeedType))
				.Description(Description)
				.AdditionalInfo(AdditionalInfo)
				.Build();
		}

		private static bool NeedActual(NormalizedUrl original, NormalizedUrl actual)
		{
			return actual != null && (actual.Tail != original.Tail || actual.Prefix != original.Prefix);
		}

		private Uri BuildOriginalUrl()
		{
			return new Uri(OriginalUrlPrefix + OriginalUrl);
		}

		private Uri BuildActualUrl()
		{
			if (HasActualUrl())
				return new Uri(ActualUrlPrefix + ActualUrl);
			return null;
		}

		private bool HasActualUrl()
		{
			return !string.IsNullOrEmpty(ActualUrlPrefix) && !string.IsNullOrEmpty(ActualUrl);
		}
	}
}