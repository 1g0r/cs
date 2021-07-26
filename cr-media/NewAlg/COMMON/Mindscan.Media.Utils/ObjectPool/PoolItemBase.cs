using System;

namespace Mindscan.Media.Utils.ObjectPool
{
	public abstract class PoolItemBase : IDisposable
	{
		private bool _disposed, _finalizer;

		~PoolItemBase()
		{
			_finalizer = true;
			ReturnToPool();
		}

		internal IResourcePool Pool;
		internal DateTime ExpiredAt;

		internal bool Expired => ExpiredAt == DateTime.MinValue || ExpiredAt < DateTime.UtcNow;
		protected internal abstract bool IsAlive { get; }
		protected internal virtual void OnReleaseResources()
		{
			//Overwrite if release actions is needed when item is removed from a pool.
		}

		public void Dispose()
		{
			_finalizer = false;
			ReturnToPool();
		}

		internal void ReleaseResources()
		{
			if (_disposed) return;

			try
			{
				if (!_finalizer)
				{
					GC.SuppressFinalize(this);
				}
				OnReleaseResources();
			}
			catch (Exception)
			{
				// Do nothing 
			}
			finally
			{
				_disposed = true;
			}
		}

		private void ReturnToPool()
		{
			if (_disposed) return;

			try
			{
				var wasReturnedToPool = Pool?.ReturnItemToPool(this) ?? false;
				if (wasReturnedToPool && _finalizer)
				{
					GC.ReRegisterForFinalize(this);
				}
			}
			catch (Exception)
			{
				ReleaseResources();
			}
		}
	}
}