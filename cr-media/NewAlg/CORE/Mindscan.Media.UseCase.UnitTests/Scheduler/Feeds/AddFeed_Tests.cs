using System;
using FluentAssertions;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Ports;
using Mindscan.Media.UseCase.Scheduler.Feeds;
using Moq;
using NUnit.Framework;

namespace Mindscan.Media.UseCase.UnitTests.Scheduler.Feeds
{
	[TestFixture]
	public class AddFeed_Tests
	{
		private ISourcesRepository _sourcesRepository;

		[OneTimeSetUp]
		public void SetUpSourceRepository()
		{
			var source = Source
				.GetBuilder().Url(new Uri("https://www.source.com"))
				.Id(1)
				.Name("fake source")
				.Build();
			var sourceRepository = new Mock<ISourcesRepository>();
			sourceRepository
				.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(source);

			_sourcesRepository = sourceRepository.Object;
		}

		[Test]
		public void Throw_If_Feed_Is_Null()
		{
			//Arrange
			var useCase = new AddFeed(null, _sourcesRepository);

			//Act
			var error = Assert.Throws<ArgumentNullException>(() => useCase.Add(1, null));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: feed");
		}

		[Test]
		public void Throw_If_Source_Not_Found()
		{
			//Arrange
			var srMoq = new Mock<ISourcesRepository>();
			srMoq.Setup(x => x.Find(It.IsAny<long>()))
				.Returns((Source) null);

			var feed = Feed.GetBuilder()
				.OriginalUrl(new Uri("http://test.com"))
				.Build();
			var useCase = new AddFeed(null, srMoq.Object);

			//Act
			var error = Assert.Throws<EntityNotFoundException>(() => useCase.Add(1, feed));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Source with id=1 not found.");
		}

		[Test]
		public void Throw_If_Repository_Returned_Null()
		{
			//Arrange
			var feedRepository = new Mock<IFeedsRepository>();
			feedRepository
				.Setup(x => x.Add(It.IsAny<long>(), It.IsAny<Feed>()))
				.Returns((Feed)null);
			var useCase = new AddFeed(feedRepository.Object, _sourcesRepository);
			var feed = Feed.GetBuilder()
				.OriginalUrl(new Uri("http://test.com"))
				.Build();


			//Act
			var error = Assert.Throws<EntityFailToAddException>(() => useCase.Add(1, feed));
			
			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Unable to add Feed.");
		}

		[Test]
		public void Success_Return_NewFeed()
		{
			//Arrange
			var newFeed = Feed.GetBuilder()
				.Id(1)
				.OriginalUrl(new Uri("http://test.com"))
				.Build();
			var feedRepository = new Mock<IFeedsRepository>();
			feedRepository
				.Setup(x => x.Add(It.IsAny<long>(), It.IsAny<Feed>()))
				.Returns(newFeed);
			var uk = new AddFeed(feedRepository.Object, _sourcesRepository);

			//Act
			var feed = uk.Add(1, newFeed);

			//Assert
			feed.Should().NotBeNull();
			feed.Should().Be(newFeed);
		}
	}
}
