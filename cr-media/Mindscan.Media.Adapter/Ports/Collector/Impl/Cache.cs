using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using Mindscan.Media.UseCase.Ports;

namespace Mindscan.Media.Adapter.Ports.Collector.Impl
{
	internal sealed class Cache<TItem> : ICache<TItem>
	{
		private readonly MemoryCache _cache;

		public Cache()
		{
			_cache = new MemoryCache("_cache." + typeof(TItem).Name);
		}
		public bool Contains(string key, out TItem item)
		{
			item = default(TItem);
			if (_cache.Contains(key))
			{
				item = (TItem)_cache[key];
				return true;
			}
			return false;
		}

		public void Add(IEnumerable<TItem> items)
		{
			throw new NotImplementedException();
		}

		public void Add(string key, TItem item)
		{
			_cache.Add(new CacheItem(key, item), new CacheItemPolicy
			{
				SlidingExpiration = TimeSpan.FromDays(5)
			});
		}

		public TItem Get(string key)
		{
			var item = _cache.GetCacheItem(key);
			if (item != null)
			{
				return (TItem) item.Value;
			}
			return default(TItem);
		}
	}
}