namespace Mindscan.Media.Utils.Logger.Impl
{
	internal sealed class LoggerFactory: ILoggerFactory
	{
		public ILogger CreateLogger(string name)
		{
			return new Logger(name);
		}
	}
}