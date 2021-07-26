using System.Collections.Generic;

namespace Mindscan.Media.Domain.Entities.Scraper
{
	public class Parser : EntityBase
	{
		private Parser()
		{
			Tests = new List<ParserTest>();
		}
		public long SourceId { get; internal set; }
		public NormalizedUrl Host { get; internal set; }
		public string Path { get; internal set; }
		public string Encoding { get; internal set; }
		public bool UseBrowser { get; internal set; }
		public string Tag { get; internal set; }
		public string Json { get; internal set; }
		public string AdditionalInfo { get; internal set; }
		public List<ParserTest> Tests { get; }

		public static ParserBuilder GetBuilder()
		{
			return new ParserBuilder(new Parser());
		}
	}
}