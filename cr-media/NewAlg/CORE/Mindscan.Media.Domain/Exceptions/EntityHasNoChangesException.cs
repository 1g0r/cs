using System;

namespace Mindscan.Media.Domain.Exceptions
{
	public class EntityHasNoChangesException : Exception
	{
		public EntityHasNoChangesException(string name, long id)
			:base($"Entity {name} with id={id} has no changes.")
		{
			
		}
	}
}