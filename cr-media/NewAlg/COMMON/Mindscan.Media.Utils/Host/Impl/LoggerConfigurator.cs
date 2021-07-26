using Mindscan.Media.Utils.Logger;
using Topshelf.Logging;

namespace Mindscan.Media.Utils.Host.Impl
{
	internal class LoggerConfigurator : HostLoggerConfigurator
	{
		private readonly ILoggerFactory _loggerFactory;
		public LoggerConfigurator(ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory;
		}
		public LogWriterFactory CreateLogWriterFactory()
		{
			return new HostLogWriterFactory(_loggerFactory);
		}
	}
}