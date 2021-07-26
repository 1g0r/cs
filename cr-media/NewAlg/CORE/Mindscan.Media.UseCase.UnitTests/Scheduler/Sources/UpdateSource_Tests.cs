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
	public class UpdateSource_Tests
	{
		[Test]
		public void Throw_If_Source_Empty()
		{
			//Arrange
			var uk = new UpdateSource(null);

			//Act
			var error = Assert.Throws<ArgumentNullException>(() => uk.Update(null));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: source");
		}

		[Test]
		public void Throw_If_Old_Not_Found()
		{
			//Arrange
			var repo = new Mock<ISourcesRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns((Source) null);
			var source = CreateBuilder().Build();
			var uk = new UpdateSource(repo.Object);

			//Act
			var error = Assert.Throws<EntityNotFoundException>(() => uk.Update(source));

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
			var uk = new UpdateSource(repo.Object);

			//Act
			var error = Assert.Throws<EntityOutdatedException>(() => uk.Update(source));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Entity 'Source' with id=1 is outdated.");
		}

		[Test]
		public void Throw_If_No_Changes()
		{
			//Arrange
			var source = CreateBuilder().Build();
			var repo = new Mock<ISourcesRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(source);
			var uk = new UpdateSource(repo.Object);

			//Act
			var error = Assert.Throws<EntityHasNoChangesException>(() => uk.Update(source));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Entity Source with id=1 has no changes.");
		}

		[Test]
		public void Throw_If_Repository_Returned_Empty()
		{
			//Arrange
			var now = DateTime.UtcNow;
			var oldSource = CreateBuilder(now).Build();
			var repo = new Mock<ISourcesRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(oldSource);
			repo.Setup(x => x.Update(It.IsAny<Source>()))
				.Returns((Source) null);
			var source = CreateBuilder(now).Name("sdfsdf").Build();
			var uk = new UpdateSource(repo.Object);

			//Act
			var error = Assert.Throws<EntityFailToUpdateException>(() => uk.Update(source));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Entity 'Source' with id=1 was not updated.");
		}

		[TestCase("https://www.source.uri", "https://www.source.uri")]
		[TestCase("https://www.source.uri", "http://www.source.uri")]
		[TestCase("https://www.source.uri", "https://source.uri")]
		[TestCase("https://www.source.uri", "http://source.uri")]
		[TestCase("https://www.source.uri", "http://source.uri/")]
		public void Should_Not_Update_Url(string oldUrl, string newUrl)
		{
			//Arrange
			var now = DateTime.UtcNow;
			var oldSource = CreateBuilder(now).Url(new Uri(oldUrl)).Build();
			var repo = new Mock<ISourcesRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(oldSource);
			repo.Setup(x => x.Update(It.IsAny<Source>()))
				.Returns((Source)null);
			var source = CreateBuilder(now).Url(new Uri(newUrl)).Build();
			var uk = new UpdateSource(repo.Object);

			//Act
			var error = Assert.Throws<EntityHasNoChangesException>(() => uk.Update(source));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Entity Source with id=1 has no changes.");
		}

		[Test]
		public void Success_Update()
		{
			//Arrange
			var now = DateTime.UtcNow;
			var oldSource = CreateBuilder(now).Build();
			var repo = new Mock<ISourcesRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(oldSource);
			var source = CreateBuilder(now).Name("sdfsdf").Build();
			repo.Setup(x => x.Update(It.IsAny<Source>()))
				.Returns(source);
			var uk = new UpdateSource(repo.Object);

			//Act
			var result = uk.Update(source);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(source);
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