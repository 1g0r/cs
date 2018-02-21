using System.Collections.Generic;

namespace ObjectPool
{
	public interface IResourcesLoader<out T> where T: PoolItemBase
	{
		IEnumerable<T> LoadResources();
		TimeSpan ExpirationTimeout { get; }
	}
}
