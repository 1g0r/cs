using System;
using System.Threading;
using FluentAssertions;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Ports;
using Mindscan.Media.UseCase.Scheduler.Triggers;
using Moq;
using NUnit.Framework;

namespace Mindscan.Media.UseCase.UnitTests.Scheduler.Triggers
{
	[TestFixture]
	public class FireTriggers_Tests : TriggersTestsBase
	{
		[Test]
		public void Throw_If_Trigger_Null()
		{
			//Arrange
			var uk = new FireTrigger(null, null);

			//Act
			var error1 = Assert.Throws<ArgumentNullException>(() => uk.Fire(null));
			var error2 = Assert.Throws<ArgumentNullException>(() => uk.FireNoCheck(null, CancellationToken.None));

			//Assert
			error1.Should().NotBeNull();
			error2.Should().NotBeNull();
			error1.Message.Should().Be("Value cannot be null.\r\nParameter name: trigger");
			error2.Message.Should().Be("Value cannot be null.\r\nParameter name: trigger");
		}

		[Test]
		public void Throw_If_Not_Found()
		{
			//Arrange
			var repo = GetTriggersRepo();
			repo.Setup(x => x.Find(It.IsAny<long>())).Returns((Trigger)null);
			var uk = new FireTrigger(repo.Object, null);
			var trigger = CreateBuilder().Build();

			//Act
			var error = Assert.Throws<EntityNotFoundException>(() => uk.Fire(trigger));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Trigger with id=1 not found.");
		}

		[Test]
		public void Throw_If_Outdated()
		{
			//Arrange
			var now = DateTime.UtcNow;
			var oldTrigger = CreateBuilder(now.AddMilliseconds(-1)).Build();
			var repo = GetTriggersRepo();
			repo.Setup(x => x.Find(It.IsAny<long>())).Returns(oldTrigger);
			var uk = new FireTrigger(repo.Object, null);
			var trigger = CreateBuilder().Build();

			//Act
			var error = Assert.Throws<EntityOutdatedException>(() => uk.Fire(trigger));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Entity 'Trigger' with id=1 is outdated.");
		}

		[Test]
		public void Success()
		{
			//Arrange
			var trigger = CreateBuilder().Build();
			var repo = GetTriggersRepo();
			repo.Setup(x => x.Find(It.IsAny<long>())).Returns(trigger);
			var starter = new Mock<ITriggerStarter>();
			var uk = new FireTrigger(repo.Object, starter.Object);

			//Act
			uk.Fire(trigger);
			uk.FireNoCheck(trigger, CancellationToken.None);
		}
	}
}