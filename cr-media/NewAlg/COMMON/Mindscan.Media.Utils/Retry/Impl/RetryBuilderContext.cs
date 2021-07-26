using System;
using System.Collections.Generic;

namespace Mindscan.Media.Utils.Retry.Impl
{
	internal sealed class RetryBuilderContext : IRetryBuilder
	{
		private readonly List<Type> _errors = new List<Type>();

		public RetryBuilderContext(Type error)
		{
			_errors.Add(error);
		}
		public IRetryBuilder For<T>() where T : Exception
		{
			_errors.Add(typeof(T));
			return this;
		}

		public IRetryManager Retry(TimeSpan retryInterval, int maxAttemptCount = 3)
		{
			return new RetryManager(_errors, retryInterval, maxAttemptCount);
		}
	}
}