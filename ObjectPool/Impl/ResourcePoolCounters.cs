using System.Threading;

namespace ObjectPool.Impl
{
	internal sealed class ResourcePoolCounters: IResourcePoolCounters
	{
		private int _createCount, 
			_destroyCount, _hitCount, 
			_notAvailableCount, _dequeueFalseCount,
			_doNotReturnCount, _returnCount,
			_resetFailCount;

		public int Count { get; internal set; }
		public int CreateCount => _createCount;
		public int DestroyCount => _destroyCount;
		public int HitCount => _hitCount;
		public int NotAvailableCount => _notAvailableCount;
		public int DequeueFalseCount => _dequeueFalseCount;
		public int DoNotReturnCount => _doNotReturnCount;
		public int ReturnCount => _returnCount;
		public int ResetFailCount => _resetFailCount;

		internal void IncCreateCount()
		{
			Interlocked.Increment(ref _createCount);
		}

		internal void IncDestroyCount()
		{
			Interlocked.Increment(ref _destroyCount);
		}

		internal void IncHitCount()
		{
			Interlocked.Increment(ref _hitCount);
		}

		internal void IncNotAvailableCount()
		{
			Interlocked.Increment(ref _notAvailableCount);
		}

		internal void IncDequeueFalseCount()
		{
			Interlocked.Increment(ref _dequeueFalseCount);
		}

		internal void IncDoNotReturnCount()
		{
			Interlocked.Increment(ref _doNotReturnCount);
		}

		internal void IncReturnCount()
		{
			Interlocked.Increment(ref _returnCount);
		}

		internal void IncResetFailCount()
		{
			Interlocked.Increment(ref _resetFailCount);
		}

		public override string ToString()
		{
			return $"Count {Count}; Created {CreateCount}; Destroyed {DestroyCount}; Hits {HitCount}; NotAvailable {NotAvailableCount}; DequeueFalse {DequeueFalseCount}; Returned {ReturnCount}";
		}
	}
}
