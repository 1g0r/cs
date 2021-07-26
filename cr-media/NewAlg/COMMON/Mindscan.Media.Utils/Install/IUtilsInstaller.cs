using Mindscan.Media.Utils.ObjectPool;

namespace Mindscan.Media.Utils.Install
{
	public interface IUtilsInstaller
	{
		IUtilsInstaller UseRoundRobinProxySwitcher();
		IUtilsInstaller UseHttpExecutor();
		IUtilsInstaller UseLogger();
		IUtilsInstaller UseHost();
		IUtilsInstaller UseDbConnector();
		IUtilsInstaller UseBroker();
		IUtilsInstaller UseRetry();
		IUtilsInstaller UseScheduler();
		IUtilsInstaller UseResourcePool<T>() where T : PoolItemBase;
	}
}