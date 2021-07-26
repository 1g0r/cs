using System;
using System.Collections.Generic;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Collector;
using Mindscan.Media.Domain.Entities.Scheduler;

namespace Mindscan.Media.UseCase.Ports
{
	public interface IMaterialsRepository
	{
		Material Add(Material material);
		IEnumerable<Tuple<Material, Source>> Find(MaterialFilter filter);
		bool Exists(Material material);
		bool Exists(NormalizedUrl url);
		IEnumerable<NormalizedUrl> FilterNotCollected(IEnumerable<NormalizedUrl> urls);
	}
}