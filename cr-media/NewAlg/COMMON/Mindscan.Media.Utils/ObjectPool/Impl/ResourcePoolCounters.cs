using System.Threading;

namespace Mindscan.Media.Utils.ObjectPool.Impl
{
	internal sealed class ResourcePoolCounters : IResourcePoolCounters
	{
		private int _createdCount,
			_destroyCount, _hitCount,
			_notAliveCount, _dequeueFalseCount,
			_doNotReturnCount, _returnCount,
			_resetFailCount;

		public int TotalCount { get; internal set; }
		public int CreatedCount => _createdCount;
		public int DestroyCount => _destroyCount;
		public int HitCount => _hitCount;
		public int NotAliveCount => _notAliveCount;
		public int DequeueFalseCount => _dequeueFalseCount;
		public int ReturnCount => _returnCount;
		public int ResetFailCount => _resetFailCount;

		internal void IncCreatedCount()
		{
			Interlocked.Increment(ref _createdCount);
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
			Interlocked.Increment(ref _notAliveCount);
		}

		internal void IncDequeueFalseCount()
		{
			Interlocked.Increment(ref _dequeueFalseCount);
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
			return $"Count {TotalCount}; Created {CreatedCount}; Destroyed {DestroyCount}; Hits {HitCount}; NotAvailable {NotAliveCount}; DequeueFalse {DequeueFalseCount}; Returned {ReturnCount}";
		}
	}
}