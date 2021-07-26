using System;
using System.Diagnostics;
using System.Threading;
using Mindscan.Media.Domain.Exceptions;

namespace Mindscan.Media.Domain.Entities
{
	[DebuggerStepThrough]
	public abstract class BuilderBase<T>
		where T : class
	{
		private volatile  int _executed = 0; 
		protected T Entity;


		protected BuilderBase(T entity)
		{
			Entity = entity;
		}

		public T Build()
		{
			if (Entity == null)
				throw new InvalidOperationException("Entity can not be null.");
			if (Interlocked.CompareExchange(ref _executed, 1, 0) == 0)
			{
				BuildInternal();
				return Entity;
			}
			throw new InvalidOperationException("Build method can be called only ones.");
		}

		protected abstract void BuildInternal();

		protected void AssertNotEmpty(string value)
		{
			if(string.IsNullOrWhiteSpace(value))
				throw new ArgumentNullException(nameof(value));
		}

		protected void AssertRequired(string value, string name)
		{
			if(string.IsNullOrWhiteSpace(value))
				throw new RequiredFieldException(name ?? nameof(value));
		}

		protected void AssertRequired(Uri value, string name)
		{
			if (value == null)
				throw new RequiredFieldException(name ?? nameof(value));
		}

		protected void AssertRequired(TimeSpan value, string name)
		{
			if(value == TimeSpan.Zero)
				throw new RequiredFieldException(name ?? nameof(value));
		}

		protected void AssertRequired(DateTime value, string name)
		{
			if (value == null || value == DateTime.MinValue || value == DateTime.MaxValue)
				throw new RequiredFieldException(name ?? nameof(value));
		}

		protected void AssertRequired(object value, string name)
		{
			if (value == null)
				throw new RequiredFieldException(name ?? nameof(value));
		}

		protected void AssertRequired(long value, string name)
		{
			if (value == 0)
				throw new RequiredFieldException(name ?? nameof(value));
		}

		protected void AssertUtc(DateTime value, string name)
		{
			if (value.Kind != DateTimeKind.Utc)
			{
				throw new NotUtcTimeZoneException(name ?? nameof(value));
			}
		}
	}
}
