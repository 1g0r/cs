using System;
using FluentAssertions;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Ports;
using Mindscan.Media.UseCase.Scheduler.Sources;
using Moq;
using NUnit.Framework;

namespace Mindscan.Media.UseCase.UnitTests.Scheduler.Sources
{
	[TestFixture]
	public class AddSource_Tests
	{
		[Test]
		public void Throw_If_Empty_Parameter()
		{
			//Arrange
			var uk = new AddSource(null);

			//Act
			var error = Assert.Throws<ArgumentNullException>(() => uk.Add(null));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: source");
		}

		[Test]
		public void Throw_If_Exist()
		{
			//Arrange
			var source = CreateBuilder().Build();
			var repo = new Mock<ISourcesRepository>();
			repo.Setup(x => x.Exists(It.IsAny<Source>()))
				.Returns(true);
			var uk = new AddSource(repo.Object);

			//Act
			var error = Assert.Throws<EntityAlreadyExistsException>(() => uk.Add(source));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Entity 'Source' with 'http://source.uri/' already exists.");
		}

		[Test]
		public void Throw_If_Repository_Returned_Null()
		{
			//Arrange
			var source = CreateBuilder().Build();
			var repo = new Mock<ISourcesRepository>();
			repo.Setup(x => x.Exists(It.IsAny<Source>()))
				.Returns(false);
			repo.Setup(x => x.Add(It.IsAny<Source>()))
				.Returns((Source) null);
			var uk = new AddSource(repo.Object);

			//Act
			var error = Assert.Throws<EntityFailToAddException>(() => uk.Add(source));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Unable to add Source.");
		}

		[Test]
		public void Success_Return_New_Source()
		{
			//Arrange
			var source = CreateBuilder().Build();
			var repo = new Mock<ISourcesRepository>();
			repo.Setup(x => x.Exists(It.IsAny<Source>()))
				.Returns(false);
			repo.Setup(x => x.Add(It.IsAny<Source>()))
				.Returns(source);
			var uk = new AddSource(repo.Object);

			//Act
			var result = uk.Add(source);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(source);
		}

		private static SourceBuilder CreateBuilder(DateTime updatedAt = default(DateTime))
		{
			if(updatedAt == default(DateTime))
				updatedAt = DateTime.UtcNow;
			return Source.GetBuilder()
				.Id(1)
				.CreatedAtUtc(updatedAt)
				.UpdatedAtUtc(updatedAt)
				.Url(new Uri("http://source.uri"))
				.Name("Source Name");
		}
	}
}
