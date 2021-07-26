using System;
using FluentAssertions;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Scheduler.Triggers;
using Moq;
using NUnit.Framework;

namespace Mindscan.Media.UseCase.UnitTests.Scheduler.Triggers
{
	[TestFixture()]
	public class UpdateTrigger_Tests: TriggersTestsBase
	{
		[Test]
		public void Throw_If_Trigger_Null()
		{
			//Arrange
			var uk = new UpdateTrigger(null);

			//Act
			var error = Assert.Throws<ArgumentNullException>(() => uk.Update(null));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: trigger");
		}

		[Test]
		public void Throw_If_Not_Found()
		{
			//Arrange
			var trigger = CreateBuilder().Build();
			var repo = GetTriggersRepo();
			repo.Setup(x => x.Find(It.IsAny<long>())).Returns((Trigger) null);
			var uk = new UpdateTrigger(repo.Object);

			//Act
			var error = Assert.Throws<EntityNotFoundException>(() => uk.Update(trigger));

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
			var uk = new UpdateTrigger(repo.Object);

			//Act
			var error = Assert.Throws<EntityOutdatedException>(() => uk.Update(CreateBuilder().Build()));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Entity 'Trigger' with id=1 is outdated.");
		}

		[Test]
		public void Throw_If_Has_No_Changes()
		{
			//Arrange
			var trigger = CreateBuilder().Build();
			var repo = GetTriggersRepo();
			repo.Setup(x => x.Find(It.IsAny<long>())).Returns(trigger);
			var uk = new UpdateTrigger(repo.Object);

			//Act
			var error = Assert.Throws<EntityHasNoChangesException>(() => uk.Update(trigger));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Entity Trigger with id=1 has no changes.");
		}

		[Test]
		public void Throw_If_Return_Null()
		{
			//Arrange
			var now = DateTime.UtcNow;
			var trigger = CreateBuilder(now).Build();
			var repo = GetTriggersRepo();
			repo.Setup(x => x.Find(It.IsAny<long>())).Returns(trigger);
			repo.Setup(x => x.Update(It.IsAny<Trigger>())).Returns((Trigger) null);
			var uk = new UpdateTrigger(repo.Object);

			//Act
			var error = Assert.Throws<EntityNotFoundException>(() => uk.Update(CreateBuilder(now).Enabled(false).Build()));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Trigger with id=1 not found.");
		}

		[Test]
		public void Success()
		{
			//Arrange
			var now = DateTime.UtcNow;
			var startAt = DateTime.UtcNow.AddHours(12);
			var oldTrigger = Trigger.GetBuilder()
				.Id(1)
				.UpdatedAtUtc(now)
				.VirtualHost("Smi")
				.RoutingKey("Smi")
				.Enabled(true)
				.RepeatInterval(new TimeSpan(0, 30, 0))
				.Payload("payload")
				.StartAtUtc(startAt)
				.Build();
			var repo = GetTriggersRepo();
			repo.Setup(x => x.Find(It.IsAny<long>())).Returns(oldTrigger);
			var newTrigger = Trigger.GetBuilder()
				.Id(1)
				.UpdatedAtUtc(now)
				.VirtualHost("Smi2")
				.RoutingKey("Smi2")
				.Enabled(false)
				.RepeatInterval(new TimeSpan(0, 6, 0))
				.Payload("payload2")
				.StartAtUtc(startAt.AddHours(-2))
				.Build();
			repo.Setup(x => x.Update(It.IsAny<Trigger>())).Returns(newTrigger);
			var uk = new UpdateTrigger(repo.Object);

			//Act
			var result = uk.Update(newTrigger);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(newTrigger);
		}
	}
}
