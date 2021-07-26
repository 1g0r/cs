using System.Net;

namespace Mindscan.Media.Utils.Proxy
{
	public interface IProxySwitcher
	{
		IWebProxy NextProxy();
	}
}
