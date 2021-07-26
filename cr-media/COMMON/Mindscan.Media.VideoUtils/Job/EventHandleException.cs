using System;

namespace Mindscan.Media.VideoUtils.Job
{
	public class EventHandleException : Exception
	{
		public EventHandleException(string message, Exception innerException) : base(message, innerException)
		{
			
		}
	}
}