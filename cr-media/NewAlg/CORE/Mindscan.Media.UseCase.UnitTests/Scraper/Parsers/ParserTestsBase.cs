using System;
using Mindscan.Media.Domain.Entities.Scraper;

namespace Mindscan.Media.UseCase.UnitTests.Scraper.Parsers
{
	public abstract class ParserTestsBase
	{
		protected static ParserBuilder CreateParserBuilder(string path = null, string tag = null)
		{
			return Parser.GetBuilder()
				.Id(1)
				.SourceId(1)
				.Host(new Uri("http://host.uri"))
				.Path(path)
				.Tag(tag);
		}
	}
}