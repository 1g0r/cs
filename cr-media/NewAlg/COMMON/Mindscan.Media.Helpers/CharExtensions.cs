namespace Mindscan.Media.Helpers
{
	public static class CharExtensions
	{
		public static bool IsWhitespace(this char symbol)
		{
			return symbol == 32 || symbol.IsInvalidWhitespace();
		}

		public static bool IsInvalidWhitespace(this char symbol)
		{
			return symbol == 160 || symbol == 8203 || symbol == 65279 || symbol == 10240;
		}
	}
}
