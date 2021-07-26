using System.Collections.Generic;
using Mindscan.Media.Domain.Entities.Scheduler;

namespace Mindscan.Media.UseCase.Ports
{
	public interface ISourcesRepository
	{
		bool Exists(Source source);
		Source Add(Source source);
		Source Find(long id);
		IEnumerable<Source> Find(SourceFilter criteria);
		int Delete(Source source);
		Source Update(Source source);
	}
}