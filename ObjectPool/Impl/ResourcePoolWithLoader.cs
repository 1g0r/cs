using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ObjectPool.Impl
{
	internal class ResourcePoolWithLoader<T> : ResourcePoolBase<T> where T : PoolItemBase
	{
		private readonly IResourcesLoader<T> _loader;
		private ConcurrentQueue<T> _loadedObjects;
		private int _loadingObjects = 0;
		private DateTime _proxyListExpirationTime;
		public ResourcePoolWithLoader(IResourcesLoader<T> loader)
		{
			_loader = loader;
		}
		public override T GetResource()
		{
			LoadObjects();
			return Get();
		}

		protected override T CreatePoolItem()
		{
			T result;
			if (_loadedObjects.TryDequeue(out result))
			{
				var copy = _proxyListExpirationTime;
				//result.ShouldReturnToPool = () => copy < DateTime.Now;
				return result;
			}
			throw new InvalidOperationException("Unable to find object in pool.");
		}

		private void LoadObjects()
		{
			if (Interlocked.CompareExchange(ref _loadingObjects, 1, 0) == 0)
			{
				if (ShouldLoadObjects())
				{
					_loadedObjects = new ConcurrentQueue<T>(_loader.LoadResources());
					//_proxyListExpirationTime = DateTime.Now.Add(TimeSpan.FromHours(1)); 
					_proxyListExpirationTime = DateTime.MaxValue; 
				}
				
				_loadingObjects = 0;
			}
		}

		private bool ShouldLoadObjects()
		{
			return _proxyListExpirationTime == DateTime.MinValue ||
			       _proxyListExpirationTime < DateTime.Now;
		}
	}
}
