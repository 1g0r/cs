using System;
using System.Threading;
using FluentAssertions;
using Mindscan.Media.Utils.Logger;
using Mindscan.Media.Utils.ObjectPool;
using Mindscan.Media.Utils.ObjectPool.Impl;
using Mindscan.Media.Utils.UnitTests.ObjectPool.Stubs;
using Moq;
using NUnit.Framework;

namespace Mindscan.Media.Utils.UnitTests.ObjectPool
{
	[Category("UnitTests")]
	[TestFixture]
	public class ObjectPoolTests
	{
		private Mock<ILoggerFactory> _loggerFactoryMock;
		private Mock<IResourcePoolConfig> _configMock;
		private readonly TimeSpan _resourcePoolTtl = new TimeSpan(0, 0, 10);
		private readonly TimeSpan _resourceRemoveTtl = new TimeSpan(0, 0, 1);

		[OneTimeSetUp]
		public void Init()
		{
			_loggerFactoryMock = new Mock<ILoggerFactory>();
			_loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>()))
				.Returns(new Mock<ILogger>().Object);

			_configMock = new Mock<IResourcePoolConfig>();
			_configMock.Setup(x => x.ResourcePoolTtl)
				.Returns(_resourcePoolTtl);
			_configMock.Setup(x => x.ResourceRemoveDelay)
				.Returns(_resourceRemoveTtl);
		}

		[TestCase(false, true)]
		[TestCase(false, false)]
		[TestCase(true, false)]
		public void ResourceLoader_ErrorOrReturnsNullOrEmpty(bool empty, bool error)
		{
			//Arrange
			var items = empty ? new PoolItemStub[0] : null; 
			var loader = new FailResourceLoaderStub(error, items);
			var pool = new ResourcePool<PoolItemStub>(loader, _configMock.Object, _loggerFactoryMock.Object);

			//Act
			Exception exception = null;
			try
			{
				var t = pool.GetResource();
			}
			catch (Exception ex)
			{
				exception = ex;
			}

			//Assert
			exception.Should().NotBeNull();
			exception.Should().BeAssignableTo<AvailableResourceNotFoundException>();
		}

		[Test]
		public void ExpiredItems_ShouldNotReturnToPool()
		{
			//Arrange 
			var item1 = new PoolItemStub();
			var item2 = new PoolItemStub();
			TimeSpan poolTtl, delay;
			poolTtl = delay = new TimeSpan(0, 0, 1);
			var loader = new ResourceLoaderStub(item1, item2);
			var pool = new ResourcePool<PoolItemStub>(loader, GetConfig(poolTtl, delay), _loggerFactoryMock.Object);

			//Act 
			using (var item = pool.GetResource())
			{
				item.Foo();
				Thread.Sleep((int)(poolTtl + delay).TotalMilliseconds);
			}
			var counters = pool.Counters;

			//Assert
			counters.Should().NotBeNull();
			counters.TotalCount.Should().Be(2);
			counters.HitCount.Should().Be(1);
			counters.DestroyCount.Should().Be(1);
			item1.ReleaseCount.Should().Be(1);
			item2.ReleaseCount.Should().Be(0);
			pool.Count.Should().Be(1);
		}

		[Test]
		public void NotAlive_ShouldNotBeUsed()
		{
			//Arrange
			var dead = new PoolItemStub(false);
			var alive = new PoolItemStub(true);
			var loader = new ResourceLoaderStub(dead, alive);
			var pool = new ResourcePool<PoolItemStub>(loader, _configMock.Object, _loggerFactoryMock.Object);

			//Act
			using (var item = pool.GetResource())
			{
				item.Foo();
			}
			var counters = pool.Counters;

			//Assert
			counters.Should().NotBeNull();
			dead.CallCount.Should().Be(0);
			dead.ReleaseCount.Should().Be(0);
			alive.CallCount.Should().Be(1);
			alive.ReleaseCount.Should().Be(0);
			counters.NotAliveCount.Should().Be(1);
			counters.TotalCount.Should().Be(2);
			counters.ReturnCount.Should().Be(2);
			pool.Count.Should().Be(2);
		}

		[Test]
		public void NotAlive_ShouldNotBeRemoved()
		{
			//Arrange
			var dead = new PoolItemStub(true);
			var alive = new PoolItemStub(true);
			var loader = new ResourceLoaderStub(dead, alive);
			var pool = new ResourcePool<PoolItemStub>(loader, _configMock.Object, _loggerFactoryMock.Object);

			//Act
			using (var item = pool.GetResource())
			{
				item.Foo();
				item.Kill();
			}
			var counters = pool.Counters;

			//Assert
			counters.Should().NotBeNull();
			dead.CallCount.Should().Be(1);
			dead.ReleaseCount.Should().Be(0);
			alive.CallCount.Should().Be(0);
			alive.ReleaseCount.Should().Be(0);
			counters.NotAliveCount.Should().Be(1);
			counters.TotalCount.Should().Be(2);
			counters.ReturnCount.Should().Be(1);
			pool.Count.Should().Be(2);
		}

		[Test]
		public void DisposePool()
		{
			//Arrange
			var dead = new PoolItemStub(false);
			var alive = new PoolItemStub(true);
			var loader = new ResourceLoaderStub(dead, alive);
			var pool = new ResourcePool<PoolItemStub>(loader, _configMock.Object, _loggerFactoryMock.Object);

			//Act
			using (var item = pool.GetResource())
			{
				item.Foo();
			}
			using (pool) { }
			var counters = pool.Counters;

			//Assert
			counters.Should().NotBeNull();
			dead.CallCount.Should().Be(0);
			dead.ReleaseCount.Should().Be(1);
			alive.CallCount.Should().Be(1);
			alive.ReleaseCount.Should().Be(1);
			counters.NotAliveCount.Should().Be(1);
			counters.TotalCount.Should().Be(2);
			counters.ReturnCount.Should().Be(2);
			counters.DestroyCount.Should().Be(2);
		}

		[Test]
		public void Resurrect()
		{
			//Arrange
			var dead = new PoolItemStub(false);
			var alive = new PoolItemStub(true);
			var loader = new ResourceLoaderStub(dead, alive);
			var pool = new ResourcePool<PoolItemStub>(loader, _configMock.Object, _loggerFactoryMock.Object);

			//Act
			using (var item = pool.GetResource())
			{
				item.Foo();
			}
			dead.Resurrect(); //It's alive!!!
			using (var item = pool.GetResource())
			{
				item.Foo();
			}
			var counters = pool.Counters;

			//Assert
			counters.Should().NotBeNull();
			dead.CallCount.Should().Be(1);
			dead.ReleaseCount.Should().Be(0);
			alive.CallCount.Should().Be(1);
			alive.ReleaseCount.Should().Be(0);
			counters.NotAliveCount.Should().Be(1);
			counters.TotalCount.Should().Be(2);
			counters.ReturnCount.Should().Be(3);
			counters.DestroyCount.Should().Be(0);
		}

		private IResourcePoolConfig GetConfig(TimeSpan poolTtl, TimeSpan deleteDelay)
		{
			var mock = new Mock<IResourcePoolConfig>();
			mock.Setup(x => x.ResourcePoolTtl).Returns(poolTtl);
			mock.Setup(x => x.ResourceRemoveDelay).Returns(deleteDelay);

			return mock.Object;
		}
	}
}
