using Mindscan.Media.Domain.Enums;

namespace Mindscan.Media.Domain.Entities.Scheduler
{
	public sealed class TriggerFilter : PagedFilter
	{
		public string VirtualHostPrefix { get; set; }
		public string RoutingKeyPrefix { get; set; }
		public TriggersFilter? TriggersFilter { get; set; }

		public string FeedOriginalUrlPrefix { get; set; }
		public string FeedActualUrlPrefix { get; set; }
		public FeedType? FeedType { get; set; }

		public string SourceUrlPrefix { get; set; }
		public SourceType? SourceType { get; set; }

		protected override void SetCustomDefaults()
		{
			FeedOriginalUrlPrefix = PrepareUrlPrefix(FeedOriginalUrlPrefix);
			FeedActualUrlPrefix = PrepareUrlPrefix(FeedActualUrlPrefix);
			SourceUrlPrefix = PrepareUrlPrefix(SourceUrlPrefix);
		}
	}
}