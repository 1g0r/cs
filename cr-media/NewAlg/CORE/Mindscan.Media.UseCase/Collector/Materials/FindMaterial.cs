using System;
using System.Collections.Generic;
using Mindscan.Media.Domain.Entities.Collector;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Collector.Materials
{
	public class FindMaterial
	{
		private readonly IMaterialsRepository _materialsRepository;

		public FindMaterial(IMaterialsRepository repository)
		{
			_materialsRepository = repository;
		}
		public IEnumerable<Tuple<Material, Source>> Find(MaterialFilter filter)
		{
			if(filter == null)
				throw new ArgumentNullException(nameof(filter));

			filter.SetDefaults();

			return _materialsRepository.Find(filter);
		}
	}
}