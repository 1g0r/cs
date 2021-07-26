using System;
using System.Collections.Generic;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Collector;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.UseCase.Collector.Materials;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.Adapter.Ports.Collector.Impl
{
	internal sealed class CollectorFacade : ICollectorFacade
	{
		private readonly Lazy<AddMaterial> _addMaterial;
		private readonly Lazy<CheckCollected> _checkCollected;
		private readonly Lazy<FindMaterial> _findMaterial;

		public CollectorFacade(
			IMaterialsRepository materialsRepository,
			ISourcesRepository sourcesRepository,
			IParsersRepository parsersRepository,
			ICache<bool> cache)
		{
			_addMaterial = new Lazy<AddMaterial>(() => new AddMaterial(materialsRepository, sourcesRepository, parsersRepository));
			_checkCollected = new Lazy<CheckCollected>(() => new CheckCollected(materialsRepository, cache));
			_findMaterial = new Lazy<FindMaterial>(() => new FindMaterial(materialsRepository));
		}
		public Material Add(Material material)
		{
			return _addMaterial.Value.Add(material);

		}

		public IEnumerable<Tuple<Material, Source>> Find(MaterialFilter filter)
		{
			return _findMaterial.Value.Find(filter);
		}

		public bool IsCollected(NormalizedUrl url)
		{
			return _checkCollected.Value.IsCollected(url);
		}

		public IEnumerable<NormalizedUrl> FilterNotCollected(IEnumerable<NormalizedUrl> urls)
		{
			return _checkCollected.Value.FilterNotCollected(urls);
		}
	}
}