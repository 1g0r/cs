using System;

namespace Mindscan.Media.Utils.ObjectPool
{
	public sealed class AvailableResourceNotFoundException : Exception
	{
		public AvailableResourceNotFoundException()
			: base("Unable to find available resource in pool.")
		{

		}
	}
}