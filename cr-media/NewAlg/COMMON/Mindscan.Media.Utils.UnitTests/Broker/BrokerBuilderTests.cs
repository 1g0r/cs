using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Mindscan.Media.Utils.Broker;
using Mindscan.Media.Utils.Broker.Impl;
using Mindscan.Media.Utils.Config;
using Mindscan.Media.Utils.Broker;
using Mindscan.Media.Utils.Logger;
using Mindscan.Media.Utils.Retry;
using Mindscan.Media.Utils.Retry.Impl;
using Moq;
using NUnit.Framework;

namespace Mindscan.Media.Utils.UnitTests.Broker
{
	[Category("UnitTests")]
	[TestFixture]
	public class BrokerBuilderTests
	{
		private IUtilsConfig _config;
		private ILoggerFactory _loggerFactory;
		private IRetryBuilder _retryBuilder;

		[OneTimeSetUp]
		public void Init()
		{
			var endpointConfig = new Mock<IBrokerEndpointConfig>().Object;
			var brokerConfigMock = new Mock<IMessageBrokerConfig>();
			brokerConfigMock.Setup(x => x.Endpoints)
				.Returns(new Dictionary<string, IBrokerEndpointConfig>
				{
					{"test", endpointConfig }
				});
			var configMock = new Mock<IUtilsConfig>();
			configMock.Setup(c => c.BrokerConfig).Returns(brokerConfigMock.Object);
			_config = configMock.Object;

			var factoryMock = new Mock<ILoggerFactory>();
			var logger = new Mock<ILogger>().Object;
			factoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger);
			_loggerFactory = factoryMock.Object;

			_retryBuilder = new RetryBuilder();
		}
		[Test]
		public void TreadSafety()
		{
			//Arrange
			IMessageBrokerBuilder builder = new MessageBrokerBuilder(_config, _loggerFactory, _retryBuilder);

			//Act
			var task1 = new Task<IMessageBroker>(() => builder.Build(CancellationToken.None, "test"));
			var task2 = new Task<IMessageBroker>(() => builder.Build(CancellationToken.None, "test"));
			var task3 = new Task<IMessageBroker>(() => builder.Build(CancellationToken.None, "test"));
			var task4 = new Task<IMessageBroker>(() => builder.Build(CancellationToken.None, "test"));

			task1.Start();
			task2.Start();
			task3.Start();
			task4.Start();
			var res1 = task1.Result;
			var res2 = task2.Result;
			var res3 = task3.Result;
			var res4 = task4.Result;

			//Assert
			res1.Should().NotBeNull();
			res2.Should().BeEquivalentTo(res1);
			res3.Should().BeEquivalentTo(res1);
			res4.Should().BeEquivalentTo(res1);
		}

		[Test]
		public void EndpointNotFound()
		{
			//Arrange
			IMessageBrokerBuilder builder = new MessageBrokerBuilder(_config, _loggerFactory, _retryBuilder);
			ConfigurationErrorsException exception = null;

			//Act 
			try
			{
				var res = builder.Build(CancellationToken.None);
			}
			catch (ConfigurationErrorsException ex)
			{
				exception = ex;
			}

			//Assert
			exception.Should().NotBeNull();
		}
	}
}
