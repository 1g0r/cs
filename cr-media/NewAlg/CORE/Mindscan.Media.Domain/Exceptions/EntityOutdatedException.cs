using System;

namespace Mindscan.Media.Domain.Exceptions
{
	public class EntityOutdatedException : Exception
	{
		public EntityOutdatedException(string name, long id)
			:base($"Entity '{name}' with id={id} is outdated.")
		{
			
		}
	}
}