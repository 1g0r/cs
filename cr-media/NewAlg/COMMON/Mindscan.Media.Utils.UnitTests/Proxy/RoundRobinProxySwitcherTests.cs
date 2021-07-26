using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Mindscan.Media.Utils.Config;
using Mindscan.Media.Utils.HttpExecutor;
using Mindscan.Media.Utils.Logger;
using Mindscan.Media.Utils.ObjectPool.Impl;
using Mindscan.Media.Utils.Proxy;
using Mindscan.Media.Utils.Proxy.Impl;
using Moq;
using NUnit.Framework;

namespace Mindscan.Media.Utils.UnitTests.Proxy
{
	[Category("UnitTests")]
	[TestFixture]
	public class RoundRobinProxySwitcherTests
	{
		private Mock<IHttpExecutor> _executorMock;
		private Mock<IUtilsConfig> _settings;
		private Mock<ILoggerFactory> _loggerFactoryMock;

		[OneTimeSetUp]
		public void Init()
		{
			_settings = new Mock<IUtilsConfig>();
			_settings.Setup(x => x.ProxyRepositoryConfig).Returns(Mock.Of<IProxyRepositoryConfig>(x => x.Debug == false));

			_loggerFactoryMock = new Mock<ILoggerFactory>();
			_loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>()))
				.Returns(new Mock<ILogger>().Object);

			_executorMock = new Mock<IHttpExecutor>();
			_executorMock
				.Setup(x => x.CreateConfigurator())
				.Returns(new HttpExecutorConfiguratorStub<string>());
			_executorMock
				.Setup(x => x.CreateConfigurator<ProxyLoader.ProxyEntity[]>())
				.Returns(new HttpExecutorConfiguratorStub<ProxyLoader.ProxyEntity[]>());

			_executorMock
				.Setup(x => x.Head(It.IsAny<Uri>(), It.IsAny<IHttpExecutorConfigurator<string>>()))
				.Returns(Mock.Of<IHttpResponse>(x => x.StatusCode == HttpStatusCode.Found));
		}

		[Test]
		public void DefaultProxy_WhenRepositoryReturnsEmptyList()
		{
			//Arrange
			_settings.Setup(x => x.ProxyRepositoryConfig).Returns(GetSwitcherSettings());
			var loader = new ProxyLoaderStub(null, _executorMock.Object, _settings.Object);
			var pool = new ResourcePool<ProxyWrapper>(
				loader, 
				_settings.Object.ProxyRepositoryConfig, 
				_loggerFactoryMock.Object);
			var switcher = new ProxySwitcher(pool);

			//Act
			var result = switcher.NextProxy();
			var result2 = switcher.NextProxy();
			var result3 = switcher.NextProxy();

			//Assert
			result.Should().Be(result2);
			result2.Should().Be(result3);
		}

		[Test]
		public void DefaultProxy_WhenDebugSetting()
		{
			//Arrange
			_settings.Setup(x => x.ProxyRepositoryConfig).Returns(GetSwitcherSettings(true));
			var loader = new ProxyLoaderStub(GetProxyList(), _executorMock.Object, _settings.Object);
			var pool = new ResourcePool<ProxyWrapper>(loader, _settings.Object.ProxyRepositoryConfig, _loggerFactoryMock.Object);
			var switcher = new ProxySwitcher(pool);

			//Act
			var result = switcher.NextProxy();
			var result2 = switcher.NextProxy();
			var result3 = switcher.NextProxy();

			//Assert
			result.Should().Be(result2);
			result2.Should().Be(result3);
		}

		[Test]
		public void RoundRobin_MainAlgorithm()
		{
			//Arrange
			_settings.Setup(x => x.ProxyRepositoryConfig).Returns(GetSwitcherSettings());
			var loader = new ProxyLoaderStub(GetProxyList(), _executorMock.Object, _settings.Object);
			var pool = new ResourcePool<ProxyWrapper>(loader, _settings.Object.ProxyRepositoryConfig, _loggerFactoryMock.Object);
			var switcher = new ProxySwitcher(pool);

			//Act
			var result1 = Task.Run(() => switcher.NextProxy());
			var result2 = Task.Run(() => switcher.NextProxy());
			var result3 = Task.Run(() => switcher.NextProxy());

			Task.WaitAll(result1, result2, result3);
			var result4 = switcher.NextProxy();

			//Assert
			result1.Result.Should().NotBeNull();
			result2.Result.Should().NotBeNull();
			result3.Result.Should().NotBeNull();
			result4.Should().NotBeNull();
			result1.Result.Should().BeEquivalentTo(result4);
		}

		[Test]
		public void RoundRobbin_ProxyListExpired()
		{
			//Arrange
			_settings.Setup(x => x.ProxyRepositoryConfig).Returns(GetSwitcherSettings(false, new TimeSpan(0, 0, 1)));
			_executorMock
				.Setup(x => x.Head(It.IsAny<Uri>(), It.IsAny<IHttpExecutorConfigurator<string>>()))
				.Returns(Mock.Of<IHttpResponse<string>>(x => x.StatusCode == HttpStatusCode.Found));

			var loader = new ProxyLoaderStub(GetProxyList(), _executorMock.Object, _settings.Object);
			var pool = new ResourcePool<ProxyWrapper>(loader, _settings.Object.ProxyRepositoryConfig, _loggerFactoryMock.Object);
			var switcher = new ProxySwitcher(pool);


			//Act
			var result1 = Task.Run(() => switcher.NextProxy());
			var result2 = Task.Run(() => switcher.NextProxy());
			Thread.Sleep(999);
			var result3 = Task.Run(() => switcher.NextProxy());
			var result4 = Task.Run(() => switcher.NextProxy());

			Task.WaitAll(result1, result2, result3, result4);

			//Assert
			result1.Result.Should().NotBeNull();
			result2.Result.Should().NotBeNull();
			result3.Result.Should().NotBeNull();
			result4.Result.Should().NotBeNull();
			result1.Result.Should().BeEquivalentTo(result4.Result);
		}

		private IProxyRepositoryConfig GetSwitcherSettings(bool debug = false, TimeSpan proxyListTtl = default(TimeSpan))
		{
			proxyListTtl = proxyListTtl == TimeSpan.Zero ? new TimeSpan(0, 30, 00) : proxyListTtl;
			return Mock.Of<IProxyRepositoryConfig>(
				x => x.Debug == debug &&
				x.ResourcePoolTtl == proxyListTtl);
		}

		private ProxyLoader.ProxyEntity[] GetProxyList()
		{
			return new[]
				{
					new ProxyLoader.ProxyEntity
					{
						Address = "127.0.0.1:1"
					},
					new ProxyLoader.ProxyEntity
					{
						Address = "127.0.0.2:2"
					},
					new ProxyLoader.ProxyEntity
					{
						Address = "127.0.0.3:3"
					}
			};
		}
	}
}