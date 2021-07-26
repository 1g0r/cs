using System;

namespace Mindscan.Media.Domain.Exceptions
{
	public class EntityFailToAddException : Exception
	{
		public EntityFailToAddException(string name)
			:base($"Unable to add {name}.")
		{
			
		}
	}
}