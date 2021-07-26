using System;

namespace Mindscan.Media.Domain.Exceptions
{
	public class ParserJsonValidationException : Exception
	{
		public ParserJsonValidationException(string message)
			:base(message)
		{
			
		}
		public ParserJsonValidationException(string message, Exception innerException)
			:base(message, innerException)
		{
			
		}
	}
}