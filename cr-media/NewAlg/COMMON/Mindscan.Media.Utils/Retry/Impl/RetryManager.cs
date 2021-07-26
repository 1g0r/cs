using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Mindscan.Media.Utils.Retry.Impl
{
	[DebuggerStepThrough]
	internal sealed class RetryManager: IRetryManager
	{
		private readonly List<Type> _errorsToHandle;
		private readonly TimeSpan _interval;
		private readonly int _maxAttemptCount;
		public RetryManager(List<Type> errors, TimeSpan interval, int maxAttemptCount)
		{
			_errorsToHandle = errors;
			_interval = interval;
			_maxAttemptCount = maxAttemptCount;
		}
		public void InvokeWithRetry(Action action)
		{
			InvokeWithRetry<int>(() =>
			{
				action();
				return 0;
			});
		}

		public T InvokeWithRetry<T>(Func<T> action)
		{
			return InvokeWithRetry(action, _errorsToHandle, _interval, _maxAttemptCount);
		}

		public static T InvokeWithRetry<T>(Func<T> action, List<Type> errorsToHandle, TimeSpan retryInterval, int maxAttemptCount)
		{
			var exceptions = new List<Exception>();
			var timeToWait = TimeSpan.Zero;
			var currentAttempt = 0;
			for (;;)
			{
				try
				{
					if (currentAttempt > 0)
					{
						timeToWait += timeToWait == TimeSpan.Zero ? retryInterval : timeToWait;
						Thread.Sleep(timeToWait);
					}
					return action();
				}
				catch (Exception ex)
				{
					if (currentAttempt < maxAttemptCount && ShouldRetry(errorsToHandle, ex))
					{
						currentAttempt++;
						if (exceptions.All(x => x.Message != ex.Message))
						{
							exceptions.Add(ex);
						}
					}
					else
					{
						if(exceptions.Count > 1)
							throw new AggregateException(exceptions);
						throw exceptions.FirstOrDefault() ?? ex;
					}
				}
			}
		}

		private static bool ShouldRetry(List<Type> errorsToHandle, Exception ex)
		{
			return errorsToHandle.Contains(ex.GetType());
		}
	}
}