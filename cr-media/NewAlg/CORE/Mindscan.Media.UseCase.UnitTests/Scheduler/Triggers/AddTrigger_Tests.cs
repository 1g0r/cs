using System;
using FluentAssertions;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Scheduler.Triggers;
using NUnit.Framework;

namespace Mindscan.Media.UseCase.UnitTests.Scheduler.Triggers
{
	[TestFixture]
	public class AddTrigger_Tests : TriggersTestsBase
	{
		[Test]
		public void Throw_If_Trigger_Is_Null()
		{
			//Arrange
			var uk = new AddTrigger(null, null, null);

			//Act
			var error = Assert.Throws<ArgumentNullException>(() => uk.Add(null));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: trigger");
		}

		[Test]
		public void Throw_If_Feed_Not_Found()
		{
			//Arrange
			var sourcesRepo = GetSourcesRepo();
			var feedRepo = GetFeedRepository(true);
			var uk = new AddTrigger(null, feedRepo, sourcesRepo);
			var trigger = CreateBuilder().Build();

			//Act
			var error = Assert.Throws<EntityNotFoundException>(() => uk.Add(trigger));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Feed with id=1 not found.");
		}

		[Test]
		public void Throw_If_Trigger_Exist()
		{
			//Arrange
			var uk = new AddTrigger(
				GetTriggersRepo(true).Object,
				GetFeedRepository(), 
				GetSourcesRepo());
			var trigger = CreateBuilder().Build();

			//Act
			var error = Assert.Throws<EntityAlreadyExistsException>(() => uk.Add(trigger));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Entity Trigger already exists.");
		}

		[Test]
		public void Throw_If_Repository_Returned_Null()
		{
			//Arrange
			var uk = new AddTrigger(
				GetTriggersRepo(returnNull:true).Object,
				GetFeedRepository(),
				GetSourcesRepo()
			);
			var trigger = CreateBuilder().Build();

			//Act
			var error = Assert.Throws<EntityFailToAddException>(() => uk.Add(trigger));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Unable to add Trigger.");
		}

		[Test]
		public void Success_New_Trigger()
		{
			//Arrange
			var uk = new AddTrigger(
				GetTriggersRepo().Object,
				GetFeedRepository(),
				GetSourcesRepo()
			);
			var trigger = CreateBuilder().Build();

			//Act
			var result = uk.Add(trigger);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(trigger);
		}
	}
}
