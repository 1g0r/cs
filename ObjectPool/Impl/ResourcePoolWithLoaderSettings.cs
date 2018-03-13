using System;

namespace ObjectPool.Impl
{
	internal sealed class ResourcePoolWithLoaderSettings: IResourcePoolWithLoaderSettings
	{
		public TimeSpan ResourcePoolTtl => new TimeSpan(12, 0, 0);
		public TimeSpan ResourceRemoveDelay => new TimeSpan(0, 0, 1);
	}
}
