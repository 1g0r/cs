using System;

namespace ObjectPool
{
	public interface IResourcePoolWithLoaderSettings
	{
		TimeSpan ResourcePoolTtl { get; }
		TimeSpan ResourceRemoveDelay { get; }
	}
}
