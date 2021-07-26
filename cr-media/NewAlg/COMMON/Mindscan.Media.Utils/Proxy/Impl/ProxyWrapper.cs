using System.Net;
using Mindscan.Media.Utils.ObjectPool;

namespace Mindscan.Media.Utils.Proxy.Impl
{
	internal class ProxyWrapper : PoolItemBase
	{
		public ProxyWrapper(IWebProxy proxy)
		{
			ProxyObject = proxy;
		}

		public IWebProxy ProxyObject { get; }
		protected internal override bool IsAlive => true;
	}
}