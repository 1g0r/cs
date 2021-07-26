using System;

namespace Mindscan.Media.Utils.Retry
{
	public interface IRetryManager
	{
		void InvokeWithRetry(Action action);
		T InvokeWithRetry<T>(Func<T> action);
	}
}