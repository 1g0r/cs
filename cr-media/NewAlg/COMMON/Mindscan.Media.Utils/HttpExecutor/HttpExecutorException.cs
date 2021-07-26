using System;

namespace Mindscan.Media.Utils.HttpExecutor
{
	public class HttpExecutorException : Exception
	{
		public HttpExecutorException(string message, Exception innerException)
			:base(message, innerException)
		{
		}
	}
}