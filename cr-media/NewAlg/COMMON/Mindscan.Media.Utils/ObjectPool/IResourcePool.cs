namespace Mindscan.Media.Utils.ObjectPool
{
	public interface IResourcePool
	{
		bool ReturnItemToPool(PoolItemBase item);
	}

	public interface IResourcePool<out T> : IResourcePool
		where T : PoolItemBase
	{
		T GetResource();
		IResourcePoolCounters Counters { get; }
	}
}
