using System;
using System.Collections.Concurrent;
using System.Threading;
using Mindscan.Media.Utils.Logger;

namespace Mindscan.Media.Utils.ObjectPool.Impl
{
	internal sealed class ResourcePool<T> : IResourcePool<T>, IDisposable
		where T : PoolItemBase
	{
		private DateTime _poolExpirationTime;
		private readonly object _locker = new object();
		private bool _initialized, _disposing;

		private readonly ConcurrentQueue<T> _resourcePool = new ConcurrentQueue<T>();
		private readonly IResourceLoader<T> _loader;
		private readonly ResourcePoolCounters _counters = new ResourcePoolCounters();
		private readonly IResourcePoolConfig _config;
		private readonly ILogger _logger;

		public ResourcePool(IResourceLoader<T> loader, IResourcePoolConfig config, ILoggerFactory factory)
		{
			_loader = loader;
			_config = config;
			_logger = factory.CreateLogger(GetType().Name);
		}
		~ResourcePool()
		{
			ReleaseResources();
		}

		public bool ReturnItemToPool(PoolItemBase item)
		{
			if (_disposing)
				return false;

			if (item.Expired)
			{
				ReleaseItem(item);
				return false;
			}
			if (!item.IsAlive)
			{
				_counters.IncNotAvailableCount();
			}
			_counters.IncReturnCount();
			_resourcePool.Enqueue((T)item); // Return item to pool	
			return true;
		}

		public IResourcePoolCounters Counters => _counters;

		public T GetResource()
		{
			LoadResourcesIfNeeded();
			var tryCount = _resourcePool.Count;
			_counters.TotalCount = tryCount;

			bool exit = false;
			do
			{
				tryCount--;
				T result = null;
				if (_resourcePool.TryDequeue(out result))
				{
					if (!result.Expired && result.IsAlive)
					{
						_counters.IncHitCount();
						return result;
					}

					ReturnItemToPool(result);
				}
				else
				{
					_counters.IncDequeueFalseCount();
					exit = true;
				}
			} while (!exit && tryCount > 0);

			throw new AvailableResourceNotFoundException();
		}

		public void Dispose()
		{
			ReleaseResources();
			GC.SuppressFinalize(this);
		}

		internal int Count => _resourcePool.Count;

		private void LoadResourcesIfNeeded()
		{
			if (!_initialized)
			{
				lock (_locker)
				{
					if (!_initialized)
					{
						LoadResources();
						_initialized = true; //Never set to false
					}
				}
			}
			else if (IsPoolExpired())
			{
				ThreadPool.QueueUserWorkItem(x => LoadResources());
			}
		}

		private void LoadResources()
		{
			try
			{
				var expiredAt = DateTime.UtcNow.Add(_config.ResourcePoolTtl).Add(_config.ResourceRemoveDelay);
				var items = _loader.LoadResources();

				if (items != null)
				{
					foreach (var item in items)
					{
						item.ExpiredAt = expiredAt;
						item.Pool = this;
						_resourcePool.Enqueue(item);
					}
					_poolExpirationTime = DateTime.UtcNow + _config.ResourcePoolTtl;
				}
			}
			catch (Exception ex)
			{
				_logger.Error("Unable to load resources {0}.", ex);
			}
		}

		private bool IsPoolExpired()
		{
			return _poolExpirationTime == DateTime.MinValue ||
					_poolExpirationTime < DateTime.UtcNow;
		}

		private void ReleaseResources()
		{
			_disposing = true;
			foreach (var item in _resourcePool)
			{
				ReleaseItem(item);
			}
		}

		private void ReleaseItem(PoolItemBase poolItem)
		{
			poolItem.ReleaseResources();
			_counters.IncDestroyCount();
		}
	}
}
