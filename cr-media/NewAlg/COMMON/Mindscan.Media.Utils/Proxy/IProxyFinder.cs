using System;
using System.Collections.Generic;
using System.Net;

namespace Mindscan.Media.Utils.Proxy
{
	public interface IProxyFinder
	{
		IWebProxy FindProxyByAddress(Uri uri);
		IEnumerable<IWebProxy> Find();
	}
}