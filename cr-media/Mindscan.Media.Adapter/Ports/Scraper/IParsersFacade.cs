using System;
using System.Collections.Generic;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Entities.Scraper;

namespace Mindscan.Media.Adapter.Ports.Scraper
{
	public interface IParsersFacade
	{
		Parser Find(long id);
		Tuple<Parser, Source> Find(NormalizedUrl pageUrl, string tag = null);
		IEnumerable<Parser> List(long sourceId);

		Parser Add(Parser parser);

		Parser Update(Parser parser);
		Parser Update(Parser parser, string json);

		string FindJson(long id);
		string ValidateJson(string json);
		string Run(string json, string content, NormalizedUrl pageUrl, bool debug);
		int Delete(Parser parser);
	}
}