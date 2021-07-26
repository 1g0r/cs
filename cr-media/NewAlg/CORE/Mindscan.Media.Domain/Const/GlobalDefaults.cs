using System;
using Mindscan.Media.Domain.Entities;

namespace Mindscan.Media.Domain.Const
{
	public static class GlobalDefaults
	{
		public const string Encoding = "utf-8";
		public const string VirtualHost = "Scheduler";
		public static string Exchange => String.Empty;
		public const string FakeSource = "http://default-source.fake/";
		public static NormalizedUrl FakeSourceRss => NormalizedUrl.Build(new Uri(FakeSource + "rss"));
	}
}
