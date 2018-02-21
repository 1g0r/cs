using System;

namespace ObjectPool
{
	public interface IResourcePoolFactory
	{
		IResourcePool<T> CreatePoolWithLoader<T>(IResourcesLoader<T> loader) where T: PoolItemBase;
		IResourcePool<T> CreatePool<T>(int minimumSize, int maximumSize, Func<T> factoryMethod) where T: PoolItemBase;
	}
}
