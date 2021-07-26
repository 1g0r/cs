using System;

namespace Mindscan.Media.Domain.Exceptions
{
	public class EntityFailToUpdateException : Exception
	{
		public EntityFailToUpdateException(string name, long id)
			: base($"Entity '{name}' with id={id} was not updated.")
		{

		}
	}
}