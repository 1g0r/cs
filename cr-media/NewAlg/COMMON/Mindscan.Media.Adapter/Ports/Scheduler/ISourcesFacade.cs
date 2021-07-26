using System.Collections.Generic;
using Mindscan.Media.Domain.Entities.Scheduler;

namespace Mindscan.Media.Adapter.Ports.Scheduler
{
	public interface ISourcesFacade
	{
		Source Add(Source source);
		Source Find(long id);
		IEnumerable<Source> Find(SourceFilter filter);
		Source Update(Source source);
		int Delete(Source source);
	}
}