namespace Mindscan.Media.Domain.Entities.Collector
{
	public sealed class MaterialFilter : PagedFilter
	{
		public string Url { get; set; }
		public string SourceUrl { get; set; }
		public string FeedUrl { get; set; }
		protected override void SetCustomDefaults()
		{
			Url = PrepareUrlPrefix(Url);
			SourceUrl = PrepareUrlPrefix(SourceUrl);
			FeedUrl = PrepareUrlPrefix(FeedUrl);
		}
	}
}