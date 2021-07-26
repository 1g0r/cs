using System;

namespace Mindscan.Media.Utils.Retry.Impl
{
	internal sealed class RetryBuilder : IRetryBuilder
	{
		public IRetryBuilder For<T>() where T : Exception
		{
			return new RetryBuilderContext(typeof(T));
		}

		public IRetryManager Retry(TimeSpan retryInterval, int maxAttemptCount = 3)
		{
			throw new NotImplementedException("At first call For<>() method");
		}
	}
}