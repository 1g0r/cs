using System.Linq;
using Mindscan.Media.Utils.Config;
using Mindscan.Media.Utils.HttpExecutor;
using Mindscan.Media.Utils.Proxy.Impl;

namespace Mindscan.Media.Utils.UnitTests.Proxy
{
	internal sealed class ProxyLoaderStub : ProxyLoader
	{
		private ProxyLoader.ProxyEntity[] _list;
		private volatile int _iteration;
		public ProxyLoaderStub(ProxyLoader.ProxyEntity[] list, IHttpExecutor executor, IUtilsConfig config) 
			: base(executor, config)
		{
			_list = list;
			_iteration = -1;
		}

		protected internal override ProxyLoader.ProxyEntity[] FetchProxyList()
		{
			_iteration++;
			if (_iteration == 0)
			{
				return _list;
			}
			_list = _list.Take(_list.Length - 1).ToArray();
			return _list;
		}
	}
}
