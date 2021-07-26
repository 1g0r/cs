using System;

namespace Mindscan.Media.Domain.Exceptions
{
	public class NotUtcTimeZoneException : ArgumentNullException
	{
		public NotUtcTimeZoneException(string name)
			: base(name, "DateTime should be in UTC time zone.")
		{

		}
	}
}