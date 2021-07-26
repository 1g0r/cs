using System.Collections.Generic;
using Mindscan.Media.Domain.Enums;

namespace Mindscan.Media.Domain.Entities.Scheduler
{
	public sealed class Feed : EntityBase
	{
		internal Feed()
		{
			Triggers = new List<Trigger>();
		}
		public NormalizedUrl OriginalUrl { get; internal set; }
		public NormalizedUrl ActualUrl { get; internal set; }
		public FeedType Type { get; internal set; }
		public string Encoding { get; internal set; }
		public string Description { get; internal set; }
		public string AdditionalInfo { get; internal set; }
		public Source Source { get; internal set; }
		public List<Trigger> Triggers { get; }

		public static FeedBuilder GetBuilder()
		{
			return new FeedBuilder();
		}

	}
}