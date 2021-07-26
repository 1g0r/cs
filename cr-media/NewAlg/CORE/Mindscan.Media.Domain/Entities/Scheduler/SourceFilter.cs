namespace Mindscan.Media.Domain.Entities.Scheduler
{
	public sealed class SourceFilter : PagedFilter
	{
		public string UrlPrefix { get; set; }
		public bool SortAsc { get; set; }

		protected override void SetCustomDefaults()
		{
			LeadingWildcard = UrlPrefix?.StartsWith("*") ?? false;
			FollowingWildcard = UrlPrefix?.EndsWith("*") ?? false;
			UrlPrefix = PrepareUrlPrefix(UrlPrefix?.Trim('*'));
		}
	}
}