using System;
using System.Collections.Generic;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Entities.Scraper;

namespace Mindscan.Media.UseCase.Ports
{
	public interface IParsersRepository
	{
		Parser Add(Parser parser);

		IEnumerable<Tuple<Parser, Source>>  Find(NormalizedUrl pageUrl, string tag);
		Parser Find(long id);
		string FindJson(long id);

		IEnumerable<Parser> List(long sourceId);

		Parser Update(Parser parser);
		Parser Update(Parser parser, string json);

		int Delete(Parser parser);
	}
}