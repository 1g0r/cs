using System;
using System.Text.RegularExpressions;

namespace Mindscan.Media.HtmlParser.Core.Helpers
{
	internal static class RegexHelper
	{
		public static Regex CreateRegex(this string pattern)
		{
			return new Regex(
				pattern, 
				RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant,
				new TimeSpan(0, 0, 1));
		}
	}
}
