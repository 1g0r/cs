using System.Collections.Generic;
using Mindscan.Media.Domain.Enums;

namespace Mindscan.Media.Domain.Entities.Scheduler
{
	public sealed class Source : EntityBase
	{
		private Source()
		{
			Feeds = new List<Feed>();
		}
		public NormalizedUrl Url { get; internal set; }
		public SourceType Type { get; internal set; }
		public string Name { get; internal set; }
		public string AdditionalInfo { get; internal set; }
		public List<Feed> Feeds { get; }

		public static SourceBuilder GetBuilder()
		{
			return new SourceBuilder(new Source());
		}
	}
}