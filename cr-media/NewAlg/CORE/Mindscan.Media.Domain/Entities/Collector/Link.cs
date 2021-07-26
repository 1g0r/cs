using System;

namespace Mindscan.Media.Domain.Entities.Collector
{
	public class Link
	{
		private Link() { }
		public Uri Url { get; internal set; }
		public string Title { get; internal set; }

		public static LinkBuilder GetBuilder()
		{
			return new LinkBuilder(new Link());
		}
	}
}