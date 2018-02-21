using System;

namespace ObjectPool
{
	public abstract class PoolItemBase : IDisposable
	{

		internal bool Disposed { get; set; }
		internal Action<PoolItemBase, bool> ReturnToPool { get; set; }

		public abstract bool IsAvailable { get; }

		private Func<bool> _shouldReturnToPool;
		internal Func<bool> ShouldReturnToPool
		{
			set { _shouldReturnToPool = value; }
			get
			{
				if (_shouldReturnToPool == null)
					return () => true;
				return _shouldReturnToPool;
			}
		}
		
		internal bool ResetState()
		{
			try
			{
				OnResetState();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal bool ReleaseResources()
		{
			try
			{
				OnReleaseResources();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		protected virtual void OnResetState()
		{
			//Overwrite if reset state actions is needed when item is returning to a pool
		}
		protected virtual void OnReleaseResources()
		{
			//Overwrite if release actions is needed when item is removed from a pool.
		}

		public void Dispose()
		{
			HandleReturnToPool(false);
		}

		~PoolItemBase()
		{
			HandleReturnToPool(true);
		}

		private void HandleReturnToPool(bool reRegisterForFinalization)
		{
			if (!Disposed)
			{
				try
				{
					ReturnToPool(this, reRegisterForFinalization);
				}
				catch (Exception)
				{
					Disposed = true;
					ReleaseResources();
				}
			}
		}
	}
}
