using System;

namespace Mindscan.Media.Domain.Exceptions
{
	public class EntityNotFoundException : Exception
	{
		public EntityNotFoundException(string message)
			:base(message)
		{
			
		}
		public EntityNotFoundException(string name, long id) 
			:base($"{name} with id={id} not found.")
		{

		}
	}
}