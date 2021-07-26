namespace Mindscan.Media.Domain.Entities.Collector
{
	public sealed class Metrics
	{
		private Metrics() { }
		public int SharesCount { get; internal set; }
		public int CommentsCount { get; internal set; }
		public int LikesCount { get; internal set; }

		public static MetricsBuilder GetBuilder()
		{
			return new MetricsBuilder(new Metrics());
		}
	}
}