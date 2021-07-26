using System;

namespace Mindscan.Media.Utils.Retry
{
	public interface IRetryBuilder
	{
		IRetryBuilder For<T>() where T : Exception;
		IRetryManager Retry(TimeSpan retryInterval, int maxAttemptCount = 3);
	}
}