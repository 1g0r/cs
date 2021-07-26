using System;
using Mindscan.Media.Utils.Config;

namespace Mindscan.Media.Utils.ObjectPool
{
	public interface IResourcePoolConfig: IConfig
	{
		TimeSpan ResourcePoolTtl { get; }
		TimeSpan ResourceRemoveDelay { get; }
	}
}