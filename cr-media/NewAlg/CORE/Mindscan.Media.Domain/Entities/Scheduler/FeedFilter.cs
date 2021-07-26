using Mindscan.Media.Domain.Enums;

namespace Mindscan.Media.Domain.Entities.Scheduler
{
	public sealed class FeedFilter : PagedFilter
	{
		public long SourceId { get; set; }
		public string OriginalUrl { get; set; }
		public string ActualUrl { get; set; }
		public FeedType? FeedType { get; set; }
		public string Encoding { get; set; }
		public TriggersFilter? TriggersFilter { get; set; }

		protected override void SetCustomDefaults()
		{
			LeadingWildcard = ActualUrl?.StartsWith("*") ?? OriginalUrl?.StartsWith("*") ?? false;
			FollowingWildcard = ActualUrl?.EndsWith("*") ?? OriginalUrl?.EndsWith("*") ?? false;
			OriginalUrl = PrepareUrlPrefix(OriginalUrl?.Trim('*'));
			ActualUrl = PrepareUrlPrefix(ActualUrl?.Trim('*'));
		}
	}
}