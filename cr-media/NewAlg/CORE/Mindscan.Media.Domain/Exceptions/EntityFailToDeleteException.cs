using System;

namespace Mindscan.Media.Domain.Exceptions
{
	public class EntityFailToDeleteException : Exception
	{
		public EntityFailToDeleteException(string name, long id)
			: base($"Fail to delete entity '{name}' with id={id}.")
		{

		}
	}
}