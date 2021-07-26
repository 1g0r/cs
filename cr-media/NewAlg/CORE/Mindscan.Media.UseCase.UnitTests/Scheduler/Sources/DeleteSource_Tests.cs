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
	public class DeleteSource_Tests
	{
		[Test]
		public void Throw_If_Empty_Parameter()
		{
			//Arrange
			var uk = new DeleteSource(null);

			//Act
			var error = Assert.Throws<ArgumentNullException>(() => uk.Delete(null));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: source");
		}

		[Test]
		public void Throw_If_Not_Exist()
		{
			//Arrange
			var source = CreateBuilder().Build();
			var repo = new Mock<ISourcesRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns((Source) null);
			var uk = new DeleteSource(repo.Object);

			//Act
			var error = Assert.Throws<EntityNotFoundException>(() => uk.Delete(source));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Source with id=1 not found.");
		}

		[Test]
		public void Throw_If_Outdated()
		{
			//Arrange
			var now = DateTime.UtcNow;
			var oldSource = CreateBuilder(now.AddMilliseconds(1)).Build();
			var repo = new Mock<ISourcesRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(oldSource);
			var source = CreateBuilder(now).Build();
			var uk = new DeleteSource(repo.Object);

			//Act
			var error = Assert.Throws<EntityOutdatedException>(() => uk.Delete(source));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Entity 'Source' with id=1 is outdated.");
		}

		[Test]
		public void Throw_If_Zero()
		{
			//Arrange
			var source = CreateBuilder().Build();
			var repo = new Mock<ISourcesRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(source);
			repo.Setup(x => x.Delete(It.IsAny<Source>()))
				.Returns(0);
			var uk = new DeleteSource(repo.Object);

			//Act
			var error = Assert.Throws<EntityFailToDeleteException>(() => uk.Delete(source));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Fail to delete entity 'Source' with id=1.");
		}

		[Test]
		public void Success_Return_One()
		{
			//Arrange
			var source = CreateBuilder().Build();
			var repo = new Mock<ISourcesRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(source);
			repo.Setup(x => x.Delete(It.IsAny<Source>()))
				.Returns(1);
			var uk = new DeleteSource(repo.Object);

			//Act
			var result = uk.Delete(source);

			//Assert
			result.Should().Be(1);
		}

		private static SourceBuilder CreateBuilder(DateTime updatedAt = default(DateTime))
		{
			if (updatedAt == default(DateTime))
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