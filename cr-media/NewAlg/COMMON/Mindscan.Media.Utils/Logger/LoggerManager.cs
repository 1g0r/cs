using Mindscan.Media.Utils.IoC.Impl;

namespace Mindscan.Media.Utils.Logger
{
	public static class LoggerManager
	{
		private static volatile ILoggerFactory _factory = Dependency.Resolver.Resolve<ILoggerFactory>();
		public static ILogger CreateLogger(string name)
		{
			return _factory.CreateLogger(name);
		}
	}
}