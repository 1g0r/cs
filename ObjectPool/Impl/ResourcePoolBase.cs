using System;
using System.Collections.Concurrent;

namespace ObjectPool.Impl
{
	internal abstract class ResourcePoolBase<T> : IResourcePool<T> where T : PoolItemBase
	{
		private readonly ConcurrentQueue<T> _resourcePool = new ConcurrentQueue<T>();
		private readonly ResourcePoolCounters _counters = new ResourcePoolCounters();
		public abstract T GetResource();
		public IResourcePoolCounters Counters => _counters;

		protected T Get()
		{
			var tryCount = _resourcePool.Count;
			_counters.Count = tryCount;
			bool exit = false;
			do
			{
				tryCount--;
				T result = null;
				if (_resourcePool.TryDequeue(out result))
				{
					if (result.IsAvailable)
					{
						_counters.IncHitCount();
						return result;
					}
					_counters.IncNotAvailableCount();	
					_resourcePool.Enqueue(result);
				}
				else
				{
					_counters.IncDequeueFalseCount();
					exit = true;
				}
			} while (!exit && tryCount > 0);

			return CreateItem();
		}

		private T CreateItem()
		{
			var result = CreatePoolItem();
			if (result != null)
			{
				_counters.IncCreateCount();
				result.ReturnToPool = ReturnItemToPool;
			}
			return result;
		}
		protected abstract T CreatePoolItem();

		private void ReturnItemToPool(PoolItemBase poolItem, bool reRegisterForFinalization)
		{
			if (poolItem.ShouldReturnToPool())
			{
				if (!poolItem.ResetState())
				{
					_counters.IncResetFailCount();
					DestroyPoolItem(poolItem);
					return;
				}
				if (reRegisterForFinalization)
				{
					GC.ReRegisterForFinalize(poolItem);
				}

				_counters.IncReturnCount();
				_resourcePool.Enqueue((T)poolItem);
			}
			else
			{
				_counters.IncDoNotReturnCount();
				DestroyPoolItem(poolItem);
			}
		}


		~ResourcePoolBase()
		{
			foreach (var item in _resourcePool)
			{
				DestroyPoolItem(item);
			}
		}

		private void DestroyPoolItem(PoolItemBase poolItem)
		{
			if (!poolItem.Disposed)
			{
				poolItem.ReleaseResources();
				poolItem.Disposed = true;
				_counters.IncDestroyCount();
			}

			GC.SuppressFinalize(poolItem);
		}
	}
}
