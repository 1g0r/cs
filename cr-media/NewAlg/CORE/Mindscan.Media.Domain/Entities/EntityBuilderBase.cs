using System;
using System.Diagnostics;

namespace Mindscan.Media.Domain.Entities
{
	[DebuggerStepThrough]
	public abstract class EntityBuilderBase<T, T2> : BuilderBase<T>
		where T : EntityBase
		where T2 : EntityBuilderBase<T, T2>
	{
		protected EntityBuilderBase(T entity): base(entity)
		{
			
		}

		public T2 Id(long value)
		{
			Entity.Id = value;
			return (T2) this;
		}

		public T2 CreatedAtUtc(DateTime value)
		{
			AssertUtc(value, nameof(CreatedAtUtc));
			Entity.CreatedAtUtc = value;
			return (T2)this;
		}

		public T2 UpdatedAtUtc(DateTime value)
		{
			AssertUtc(value, nameof(UpdatedAtUtc));
			Entity.UpdatedAtUtc = value;
			return (T2)this;
		}
	}
}