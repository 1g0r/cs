using System.Collections.Generic;
using Mindscan.Media.Utils.ObjectPool;

namespace Mindscan.Media.Utils.UnitTests.ObjectPool.Stubs
{
	internal sealed class ResourceLoaderStub : IResourceLoader<PoolItemStub>
	{
		private readonly PoolItemStub[] _items;
		private int _count;
		public ResourceLoaderStub(params PoolItemStub[] items)
		{
			_items = items;
		}
		public IEnumerable<PoolItemStub> LoadResources()
		{
			_count++;
			return _items;

		}

		public int Count => _count;
	}
}