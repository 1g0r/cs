namespace ObjectPool
{
	public interface IResourcePoolCounters
	{
		int Count { get; }
		int CreateCount { get; }
		int DestroyCount { get; }
		int HitCount { get; }
		int NotAvailableCount { get; }
		int DequeueFalseCount { get; }
		int DoNotReturnCount { get; }
		int ReturnCount { get; }
		int ResetFailCount { get; }
	}
}
