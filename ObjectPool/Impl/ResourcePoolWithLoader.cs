using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ObjectPool.Impl
{
	internal class ResourcePoolWithLoader<T> : ResourcePoolBase<T> where T : PoolItemBase
	{
		private readonly IResourcesLoader<T> _loader;
		private readonly IResourcePoolWithLoaderSettings _settings;
		private int _initialized = 0, _loading = 0;
		private DateTime _proxyListExpirationTime;

		public ResourcePoolWithLoader(IResourcesLoader<T> loader, IResourcePoolWithLoaderSettings settings)
		{
			_loader = loader;
			_settings = settings;
		}
		public override T GetResource()
		{
			Load();
			return Get();
		}

		protected override T CreatePoolItem()
		{
			throw new InvalidOperationException("Unable to find object in pool.");
		}

		private void Load()
		{
			if (Interlocked.CompareExchange(ref _loading, 1, 0) == 0)
			{
				if (IsPoolExpired())
				{
					_proxyListExpirationTime = DateTime.Now.Add(_settings.ResourcePoolTtl);
					if (Interlocked.CompareExchange(ref _initialized, 1, 0) == 0)
					{
						LoadResources();
					}
					else
					{
						ThreadPool.QueueUserWorkItem(x => LoadResources());
					}
				}
				_loading = 0; //Release lock
			}
		}

		private void LoadResources()
		{
			var items = _loader.LoadResources();
			if (items != null)
			{
				foreach (var item in items)
				{
					var copy = _proxyListExpirationTime - _settings.ResourceRemoveDelay;
					item.ShouldReturnToPool = () => copy > DateTime.Now;
					AddItem(item);
				}
			}
		}

		private bool IsPoolExpired()
		{
			return _proxyListExpirationTime == DateTime.MinValue ||
				   _proxyListExpirationTime < DateTime.Now;
		}
	}
}
