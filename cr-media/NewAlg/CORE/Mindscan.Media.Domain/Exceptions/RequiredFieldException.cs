using System;

namespace Mindscan.Media.Domain.Exceptions
{
	public class RequiredFieldException : ArgumentNullException
	{
		public RequiredFieldException(string name) : base(name)
		{

		}
	}
}