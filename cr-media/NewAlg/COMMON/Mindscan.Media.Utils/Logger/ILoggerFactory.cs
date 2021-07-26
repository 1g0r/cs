namespace Mindscan.Media.Utils.Logger
{
	public interface ILoggerFactory
	{
		ILogger CreateLogger(string name);
	}
}
