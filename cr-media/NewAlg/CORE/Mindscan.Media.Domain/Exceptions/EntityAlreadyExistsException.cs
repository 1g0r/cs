using System;

namespace Mindscan.Media.Domain.Exceptions
{
	public class EntityAlreadyExistsException : Exception
	{
		public EntityAlreadyExistsException(string name) 
			: base($"Entity {name} already exists.")
		{

		}

		public EntityAlreadyExistsException(string name, string with)
			: base($"Entity '{name}' with '{with}' already exists.")
		{

		}
	}
}
