using System;
using System.Collections.Generic;
using System.Linq;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Collector;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.UseCase.Collector.Materials
{
	public class CheckCollected
	{
		private readonly IMaterialsRepository _materialsRepository;
		private readonly ICache<bool> _urlCache;

		public CheckCollected(IMaterialsRepository materialsRepository, ICache<bool> cache)
		{
			_materialsRepository = materialsRepository;
			_urlCache = cache;
		}
		public IEnumerable<NormalizedUrl> FilterNotCollected(IEnumerable<NormalizedUrl> urls)
		{
			if(urls == null)
				throw new ArgumentNullException(nameof(urls));

			bool result;
			var notInCache = urls
				.Distinct()
				.Where(x => !_urlCache.Contains(x.Tail, out result))
				.ToList();

			var notInDb = _materialsRepository
				.FilterNotCollected(notInCache)
				.ToList();

			// All urls that are not in cache and are in DB must be added to cache.
			foreach (var item in notInCache.Where(x => !notInDb.Contains(x)))
			{
				_urlCache.Add(item.Tail, true);
			}

			return notInDb;
		}

		public bool IsCollected(NormalizedUrl url)
		{
			if(url == null)
				throw new ArgumentNullException(nameof(url));

			bool result;
			if (_urlCache.Contains(url.Tail, out result))
			{
				return result;
			}

			if (_materialsRepository.Exists(url))
			{
				_urlCache.Add(url.Tail, true);
				return true;
			}
			
			return false;
		}
	}
}