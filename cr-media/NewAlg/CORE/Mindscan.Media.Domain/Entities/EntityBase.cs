using System;

namespace Mindscan.Media.Domain.Entities
{
	public abstract class EntityBase
	{
		public long Id { get; internal set; }
		public DateTime CreatedAtUtc { get; internal set; }
		public DateTime UpdatedAtUtc { get; internal set; }
	}
}