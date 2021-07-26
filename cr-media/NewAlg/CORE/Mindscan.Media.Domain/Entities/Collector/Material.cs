using System;
using System.Collections.Generic;

namespace Mindscan.Media.Domain.Entities.Collector
{
	public sealed class Material : EntityBase
	{
		private Material() { }

		public long SourceId { get; internal set; }
		public long? ParserId { get; internal set; }
		public NormalizedUrl OriginalUrl { get; internal set; }
		public NormalizedUrl ActualUrl { get; internal set; }
		public NormalizedUrl FeedUrl { get; internal set; }
		public string Title { get; internal set; }
		public string Text { get; internal set; }
		public NormalizedUrl Host { get; internal set; }
		public DateTime PublishedAtUtc { get; internal set; }

		//LISTS
		public List<Author> Authors { get; } = new List<Author>();
		public List<string> Tags { get; } = new List<string>();
		public List<Link> Images { get; } = new List<Link>();
		public List<Link> Links { get; } = new List<Link>();
		public List<string> Categories { get; } = new List<string>();
		public List<Link> Videos { get; } = new List<Link>();
		public List<Link> Pdfs { get; } = new List<Link>();
		public Metrics Metrics { get; internal set; }

		public static MaterialBuilder GetBuilder()
		{
			return new MaterialBuilder(new Material());
		}
	}
}
