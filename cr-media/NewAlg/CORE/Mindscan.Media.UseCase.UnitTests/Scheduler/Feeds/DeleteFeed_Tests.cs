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
	public class DeleteFeed_Tests
	{
		[Test]
		public void Throw_If_Feed_Is_Null()
		{
			//Arrange
			var feedRepo = new Mock<IFeedsRepository>();
			var uk = new DeleteFeed(feedRepo.Object);

			//Act
			var error = Assert.Throws<ArgumentNullException>(() => uk.Delete(null));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: feed");
		}

		[Test]
		public void Throw_If_Old_Not_Found()
		{
			//Arrange
			var feedRepo = new Mock<IFeedsRepository>();
			feedRepo
				.Setup(x => x.Find(It.IsAny<long>()))
				.Returns((Feed)null);
			var feed = Feed.GetBuilder()
				.Id(1)
				.OriginalUrl(new Uri("http://feed.com"))
				.Build();
			var uk = new DeleteFeed(feedRepo.Object);

			//Act
			var error = Assert.Throws<EntityNotFoundException>(() => uk.Delete(feed));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Feed with id=1 not found.");
		}

		[Test]
		public void Throw_If_Outdated()
		{
			//Arrange
			var now = DateTime.UtcNow;
			var oldFeed = Feed.GetBuilder()
				.Id(1)
				.OriginalUrl(new Uri("http://feed.com"))
				.CreatedAtUtc(now)
				.UpdatedAtUtc(now.AddSeconds(-1))
				.Build();
			var feedRepo = new Mock<IFeedsRepository>();
			feedRepo
				.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(oldFeed);
			var feed = Feed.GetBuilder()
				.Id(1)
				.OriginalUrl(new Uri("http://feed.com"))
				.CreatedAtUtc(now)
				.UpdatedAtUtc(now)
				.Build();
			var uk = new DeleteFeed(feedRepo.Object);

			//Act
			var error = Assert.Throws<EntityOutdatedException>(() => uk.Delete(feed));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Entity 'Feed' with id=1 is outdated.");
		}

		[Test]
		public void Throw_If_Zero()
		{
			//Arrange
			var feedBuilder = Feed.GetBuilder()
				.Id(1)
				.OriginalUrl(new Uri("http://feed.com"))
				.Build();

			var repo = new Mock<IFeedsRepository>();
			repo.Setup(x => x.Delete(It.IsAny<Feed>()))
				.Returns(0);
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(feedBuilder);
			var uk = new DeleteFeed(repo.Object);

			//Act
			var error = Assert.Throws<EntityFailToDeleteException>(() => uk.Delete(feedBuilder));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Fail to delete entity 'Feed' with id=1.");
		}

		[Test]
		public void Success_Return_One()
		{
			//Assert 
			var feed = Feed.GetBuilder()
				.Id(1)
				.OriginalUrl(new Uri("http://feed.com"))
				.CreatedAtUtc(DateTime.UtcNow)
				.UpdatedAtUtc(DateTime.UtcNow)
				.Build();
			var feedRepo = new Mock<IFeedsRepository>();
			feedRepo
				.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(feed);
			feedRepo.Setup(x => x.Delete(It.IsAny<Feed>()))
				.Returns(1);
			var uk = new DeleteFeed(feedRepo.Object);

			//Act
			var num = uk.Delete(feed);

			//Assert
			num.Should().Be(1);
		}
	}
}