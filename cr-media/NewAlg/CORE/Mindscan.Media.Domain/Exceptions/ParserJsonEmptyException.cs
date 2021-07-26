using System;

namespace Mindscan.Media.Domain.Exceptions
{
	public class ParserJsonEmptyException : Exception
	{
		public ParserJsonEmptyException(Uri pageUrl)
			:base($"Parser has empty JSON for the page='{pageUrl}'.")
		{
			
		}

		public ParserJsonEmptyException(Uri pageUrl, string tag)
			: base($"Parser has empty JSON for the URL='{pageUrl}' and TAG='{tag}'.")
		{
			
		}
	}
}