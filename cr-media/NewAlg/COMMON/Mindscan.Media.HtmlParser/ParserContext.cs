using System;

namespace Mindscan.Media.HtmlParser
{
	public class ParserContext
	{
		public Uri PageUrl { get; }
		public bool Debug { get; }
		internal ParserContext(Uri pageUrl, bool debug)
		{
			PageUrl = pageUrl;
			Debug = debug;
		}
	}
}