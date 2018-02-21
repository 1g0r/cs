namespace ObjectPool
{
	public interface IResourcePool<out T> where T : PoolItemBase
	{
		T GetResource();
		IResourcePoolCounters Counters { get; }
	}
}
