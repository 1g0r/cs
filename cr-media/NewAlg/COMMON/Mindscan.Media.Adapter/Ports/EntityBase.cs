using System;

namespace Mindscan.Media.Adapter.Ports
{
	internal abstract class EntityBase
	{
		public long Id { get; set; }
		public DateTime CreatedAtUtc { get; set; }
		public DateTime UpdatedAtUtc { get; set; }
	}
}
