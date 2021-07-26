using System.Collections.Generic;
using Mindscan.Media.Utils.ObjectPool;

namespace Mindscan.Media.Utils.UnitTests.ObjectPool.Stubs
{
	internal sealed class FailResourceLoaderStub : IResourceLoader<PoolItemStub>
	{
		private readonly bool _throwError;
		private readonly PoolItemStub[] _items;
		public FailResourceLoaderStub(bool throwError, PoolItemStub[] items)
		{
			_throwError = throwError;
			_items = items;
		}
		public IEnumerable<PoolItemStub> LoadResources()
		{
			if(_throwError)
				throw new System.NotImplementedException();
			return _items;
		}
	}
}