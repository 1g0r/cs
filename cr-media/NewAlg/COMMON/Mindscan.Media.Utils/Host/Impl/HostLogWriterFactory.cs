using Mindscan.Media.Utils.Logger;
using Topshelf.Logging;

namespace Mindscan.Media.Utils.Host.Impl
{
	internal class HostLogWriterFactory : LogWriterFactory
	{
		private readonly ILoggerFactory _loggerFactory;

		public HostLogWriterFactory(ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory;
		}
		public LogWriter Get(string name)
		{
			return new HostLogWriter(_loggerFactory.CreateLogger(name));
		}

		public void Shutdown()
		{
			
		}
	}
}