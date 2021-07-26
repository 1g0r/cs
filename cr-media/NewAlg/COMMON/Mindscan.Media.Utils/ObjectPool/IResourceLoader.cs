using System.Collections.Generic;

namespace Mindscan.Media.Utils.ObjectPool
{
	public interface IResourceLoader<out T> where T : PoolItemBase
	{
		IEnumerable<T> LoadResources();
	}
}