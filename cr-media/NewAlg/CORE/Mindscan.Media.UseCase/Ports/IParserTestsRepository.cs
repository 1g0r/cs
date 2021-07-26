using System;
using System.Collections.Generic;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Scraper;

namespace Mindscan.Media.UseCase.Ports
{
	public interface IParserTestsRepository
	{
		ParserTest Add(long parserId, ParserTest test);
		IEnumerable<ParserTest> List(long parserId);
		ParserTest Get(NormalizedUrl url);
		ParserTest Get(long id);
		ParserTest Update(long id, DateTime updatedAtUtc, ParserTest test);
		int Delete(int id, DateTime updatedAtUtc);
	}
}