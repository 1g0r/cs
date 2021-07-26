using System.Text;

namespace Mindscan.Media.Helpers
{
	public static class StringExtensions
	{
		public static string ClearString(this string rawValue)
		{
			if (string.IsNullOrEmpty(rawValue))
				return null;

			var result = new StringBuilder(rawValue.Length);
			for (int i = 0, l = rawValue.Length; i < l; ++i)
			{
				var symbol = rawValue[i];

				if (ShouldSkipLeadingWhitespace(result.Length, symbol))
					continue;

				if (symbol == '\r' || symbol == '\n' || symbol == '\t')
				{
					if (result.Length != 0 && !IsLastSymbolWhitespace(result))
					{
						result.Append(" ");
					}
					continue;
				}

				if (IsLastSymbolWhitespace(result) && symbol.IsWhitespace())
					continue;

				if (symbol.IsInvalidWhitespace())
				{
					result.Append(" ");
					continue;
				}

				result.Append(symbol);
			}

			if (IsLastSymbolWhitespace(result))
			{
				result.Remove(result.Length - 1, 1);
			}
			var str = result.ToString();
			return string.IsNullOrEmpty(str) ? null : str;
		}

		private static bool ShouldSkipLeadingWhitespace(int resultLength, char symbol)
		{
			return resultLength == 0 && symbol.IsWhitespace();
		}

		private static bool IsLastSymbolWhitespace(StringBuilder result)
		{
			return result.Length != 0 && result[result.Length - 1].IsWhitespace();
		}

	}
}
