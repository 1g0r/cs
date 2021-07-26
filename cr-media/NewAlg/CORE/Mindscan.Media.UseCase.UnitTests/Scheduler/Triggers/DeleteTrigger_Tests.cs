using System;
using FluentAssertions;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Scheduler.Triggers;
using Moq;
using NUnit.Framework;

namespace Mindscan.Media.UseCase.UnitTests.Scheduler.Triggers
{
	[TestFixture]
	public class DeleteTrigger_Tests : TriggersTestsBase
	{
		[Test]
		public void Throw_If_Trigger_Null()
		{
			//Arrange
			var uk = new DeleteTrigger(null);

			//Act
			var error = Assert.Throws<ArgumentNullException>(() => uk.Delete(null));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: trigger");
		}

		[Test]
		public void Throw_If_Not_Found()
		{
			//Arrange
			var uk = new DeleteTrigger(
				GetTriggersRepo(returnNull:true).Object
			);
			var trigger = CreateBuilder().Build();

			//Act
			var error = Assert.Throws<EntityNotFoundException>(() => uk.Delete(trigger));

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
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(oldTrigger);
			var uk = new DeleteTrigger(repo.Object);

			//Act 
			var error = Assert.Throws<EntityOutdatedException>(() => uk.Delete(CreateBuilder().Build()));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Entity 'Trigger' with id=1 is outdated.");
		}

		[Test]
		public void Throw_If_Zero()
		{
			//Arrange
			var trigger = CreateBuilder().Build();
			var repo = GetTriggersRepo(true);
			repo.Setup(x => x.Find(It.IsAny<long>())).Returns(trigger);
			repo.Setup(x => x.Delete(It.IsAny<Trigger>())).Returns(0);
			var uk = new DeleteTrigger(repo.Object);

			//Act
			var error = Assert.Throws<EntityFailToDeleteException>(() => uk.Delete(trigger));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Fail to delete entity 'Trigger' with id=1.");
		}

		[Test]
		public void Success_Return_One()
		{
			//Arrange
			var trigger = CreateBuilder().Build();
			var repo = GetTriggersRepo(true);
			repo.Setup(x => x.Find(It.IsAny<long>())).Returns(trigger);
			repo.Setup(x => x.Delete(It.IsAny<Trigger>())).Returns(1);
			var uk = new DeleteTrigger(repo.Object);

			//Act
			var result = uk.Delete(trigger);

			//Assert
			result.Should().Be(1);
		}
	}
}