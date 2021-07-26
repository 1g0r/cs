using Mindscan.Media.Utils.ObjectPool;

namespace Mindscan.Media.Utils.UnitTests.ObjectPool.Stubs
{
	public class PoolItemStub : PoolItemBase
	{
		private bool _isAlive;
		public PoolItemStub(bool isAlive = true)
		{
			_isAlive = isAlive;
		}
		public int CallCount;
		public int ReleaseCount;
		protected internal override bool IsAlive => _isAlive;

		public void Foo()
		{
			CallCount++;
		}

		public void Kill()
		{
			_isAlive = false;
		}

		public void Resurrect()
		{
			_isAlive = true;
		}
		protected internal override void OnReleaseResources()
		{
			ReleaseCount++;
		}
	}
}