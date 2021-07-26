using System;
using Mindscan.Media.Utils.Config;

namespace Mindscan.Media.Utils.Host
{
	public interface IHostConfig : IConfig
	{
		TimeSpan StartTimeout { get; }
		TimeSpan StopTimeout { get; }
		string Name { get; }
		string DisplayName { get; }
		string Description { get; }
		string InstanceName { get; }
		string User { get; }
		string Password { get; }
	}
}
