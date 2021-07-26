using System;

namespace Mindscan.Media.Domain.Entities.Scraper
{
	public class ParserTest : EntityBase
	{
		private ParserTest()
		{
			
		}
		public long ParserId { get; internal set; }
		public NormalizedUrl PageUrl { get; internal set; }
		public bool Enabled { get; internal set; }
		public string ResultJson { get; internal set; }
		public bool Passed { get; internal set; }
		public DateTime? LastPassedAtUtc { get; internal set; }
		public int ExecutionsCount { get; internal set; }

		public static ParserTestBuilder GetBuilder()
		{
			return new ParserTestBuilder(new ParserTest());
		}
	}
}