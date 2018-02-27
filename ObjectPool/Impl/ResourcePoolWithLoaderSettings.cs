using System;

namespace ObjectPool.Impl
{
	public interface IResourcePoolWithLoaderSettings
	{
		TimeSpan ResourcePoolTtl { get; }
		TimeSpan ResourceRemoveDelay { get; }
	}

	internal sealed class ResourcePoolWithLoaderSettings: IResourcePoolWithLoaderSettings
	{
		public TimeSpan ResourcePoolTtl => new TimeSpan(4, 0, 0);
		public TimeSpan ResourceRemoveDelay => new TimeSpan(0, 0, 1);
	}
}
