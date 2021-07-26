using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Mindscan.Media.HtmlParser.Core.Helpers
{
	internal static class MnemonicHelper
	{
		private static readonly Regex Mnemonic = @"(&amp;)|(&lt;)|(&gt;)|(&apos;)|(&quot;)".CreateRegex();
		private static readonly Regex Name = @"(\w)".CreateRegex();

		public static string Decode(this string value)
		{
			if (string.IsNullOrEmpty(value))
				return value;
			var builder = new StringBuilder();
			foreach (var str in Mnemonic.Split(value))
			{
				builder.Append(Mnemonic.IsMatch(str) ? str : WebUtility.HtmlDecode(str));
			}

			return builder.ToString();
		}

		public static bool IsXmlMnemonic(this string value)
		{
			return Mnemonic.IsMatch(value);
		}

		public static bool IsMnemonicName(this char value)
		{
			return Name.IsMatch(value.ToString());
		}
	}
}