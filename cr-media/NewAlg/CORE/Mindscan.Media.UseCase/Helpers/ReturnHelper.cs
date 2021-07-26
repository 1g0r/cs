using System;
using System.Collections.Generic;
using System.Linq;

namespace Mindscan.Media.UseCase.Helpers
{
	internal static class ReturnHelper
	{
		public static T ThrowIfNull<T>(this T value, Func<Exception> thrower) where T : class
		{
			if (value == null)
				throw thrower();
			return value;
		}

		public static IEnumerable<T> EnsureNotNull<T>(this IEnumerable<T> values)
		{
			if (values == null)
				return Enumerable.Empty<T>();
			return values;
		}

		public static long ThrowIfZero(this long value, Func<Exception> thrower) 
		{
			if (value == 0)
				throw thrower();

			return value;
		}

		public static int ThrowIfZero(this int value, Func<Exception> thrower)
		{
			if (value == 0)
				throw thrower();

			return value;
		}
	}
}