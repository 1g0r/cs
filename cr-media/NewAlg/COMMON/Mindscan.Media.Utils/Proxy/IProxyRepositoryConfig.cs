using System;
using Mindscan.Media.Utils.ObjectPool;

namespace Mindscan.Media.Utils.Proxy
{
	public interface IProxyRepositoryConfig: IResourcePoolConfig
	{
		Uri ProxyListUri { get; }
	}
}