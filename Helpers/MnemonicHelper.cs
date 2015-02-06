public static class MnemonicHelper
	{
		private static readonly Regex Mnemonic = new Regex(@"(&amp;)|(&lt;)|(&gt;)|(&apos;)|(&quot;)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		private static readonly Regex Name = new Regex(@"(\w)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public static string Decode(this string value)
		{
			if (value.IsNullOrEmpty())
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
