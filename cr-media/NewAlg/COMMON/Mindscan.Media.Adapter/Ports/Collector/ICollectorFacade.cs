using System;
using System.Collections.Generic;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Collector;
using Mindscan.Media.Domain.Entities.Scheduler;

namespace Mindscan.Media.Adapter.Ports.Collector
{
	public interface ICollectorFacade
	{
		Material Add(Material material);
		IEnumerable<Tuple<Material, Source>> Find(MaterialFilter filter);
		bool IsCollected(NormalizedUrl url);
		IEnumerable<NormalizedUrl> FilterNotCollected(IEnumerable<NormalizedUrl> urls);
	}
}
