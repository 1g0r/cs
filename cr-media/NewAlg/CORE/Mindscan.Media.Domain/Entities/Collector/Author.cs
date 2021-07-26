namespace Mindscan.Media.Domain.Entities.Collector
{
	public sealed class Author
	{
		private Author() { }
		public string Name { get; internal set; }
		public NormalizedUrl Url { get; internal set; }

		public static AuthorBuilder GetBuilder()
		{
			return new AuthorBuilder(new Author());
		}
	}
}
