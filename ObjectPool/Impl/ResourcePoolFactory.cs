using System;

namespace ObjectPool.Impl
{
	internal class ResourcePoolFactory: IResourcePoolFactory
	{
		IResourcePool<T> IResourcePoolFactory.CreatePoolWithLoader<T>(IResourcesLoader<T> loader)
		{
			return new ResourcePoolWithLoader<T>(loader, new ResourcePoolWithLoaderSettings());
		}

		IResourcePool<T> IResourcePoolFactory.CreatePool<T>(int minimumSize, int maximumSize, Func<T> factoryMethod)
		{
			throw new NotImplementedException();
		}
	}
}
