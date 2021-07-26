namespace Mindscan.Media.Utils.ObjectPool
{
	public interface IResourcePoolCounters
	{
		int TotalCount { get; }
		int CreatedCount { get; }
		int DestroyCount { get; }
		int HitCount { get; }
		int NotAliveCount { get; }
		int DequeueFalseCount { get; }
		int ReturnCount { get; }
		int ResetFailCount { get; }
	}
}