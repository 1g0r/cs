using System;

namespace ObjectPool
{
	public sealed class AvailableResourceNotFoundException : Exception
	{
		public AvailableResourceNotFoundException()
			:base("Unable to find available resource in pool.")
		{
			
		}
	}
}
