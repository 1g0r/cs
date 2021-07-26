using System;
using System.Collections.Generic;
using System.Linq;
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
	public class FindSource_Tests
	{
		[Test]
		public void Throw_If_Not_Found()
		{
			//Arrange
			var repo = new Mock<ISourcesRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns((Source) null);
			var uk = new FindSource(repo.Object);

			//Act
			var error = Assert.Throws<EntityNotFoundException>(() => uk.Find(1));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Source with id=1 not found.");
		}

		[Test]
		public void Success_Return_Source()
		{
			//Arrange
			var source = CreateBuilder().Build();
			var repo = new Mock<ISourcesRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(source);
			var uk = new FindSource(repo.Object);

			//Act
			var result = uk.Find(1);

			//assert
			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(source);
		}

		[Test]
		public void Throw_If_Empty_Filter()
		{
			//Arrange
			var uk = new FindSource(null);

			//Act
			var error = Assert.Throws<ArgumentNullException>(() => uk.Find(null));

			//Arrange
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: filter");
		}

		[Test]
		public void Return_Empty_Enumerable()
		{
			//Arrange
			var repo = new Mock<ISourcesRepository>();
			repo.Setup(x => x.Find(It.IsAny<SourceFilter>()))
				.Returns((IEnumerable<Source>)null);
			var uk = new FindSource(repo.Object);

			//Act
			var result = uk.Find(new SourceFilter());

			//Assert
			result.Should().NotBeNull();
			result.Count().Should().Be(0);
		}

		[Test]
		public void Success_Find_Sources()
		{
			//Arrange
			var sources = new List<Source>
			{
				CreateBuilder().Build(),
				CreateBuilder().Build()
			};
			var repo = new Mock<ISourcesRepository>();
			repo.Setup(x => x.Find(It.IsAny<SourceFilter>()))
				.Returns(sources);
			var uk = new FindSource(repo.Object);

			//Act
			var result = uk.Find(new SourceFilter());

			//Assert
			result.Should().NotBeNull();
			result.Count().Should().Be(2);
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