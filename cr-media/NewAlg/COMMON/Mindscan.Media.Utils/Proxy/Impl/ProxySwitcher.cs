using System.Net;
using Mindscan.Media.Utils.ObjectPool;

namespace Mindscan.Media.Utils.Proxy.Impl
{
	internal class ProxySwitcher : IProxySwitcher
	{
		private readonly IResourcePool<ProxyWrapper> _proxyPool;
		public ProxySwitcher(IResourcePool<ProxyWrapper> proxyPool)
		{
			_proxyPool = proxyPool;
		}
		public IWebProxy NextProxy()
		{
			using (var resource = _proxyPool.GetResource())
			{
				return resource.ProxyObject;
			}
		}
	}
}
