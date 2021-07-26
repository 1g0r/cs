using System;
using System.Collections.Generic;
using System.Linq;
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
	public class FindFeed_Tests
	{
		[Test]
		public void Throw_If_Not_Found()
		{
			//Arrange
			var repo = new Mock<IFeedsRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns((Feed) null);
			var uk = new FindFeed(repo.Object);

			//Act
			var error = Assert.Throws<EntityNotFoundException>(() => uk.Find(1));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Feed with id=1 not found.");
		}

		[Test]
		public void Success_Return_Feed()
		{
			//Arrange
			var feed = Feed.GetBuilder()
				.Id(1)
				.OriginalUrl(new Uri("http://feed.com"))
				.Build();
			var repo = new Mock<IFeedsRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(feed);
			var uk = new FindFeed(repo.Object);

			//Act
			var result = uk.Find(1);

			//Assert
			result.Should().NotBeNull();
		}

		[Test]
		public void Throw_If_Empty_Filter()
		{
			//Arrange
			var uk = new FindFeed(null);

			//Act
			var error = Assert.Throws<ArgumentNullException>(() => uk.Find(null));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: filter");
		}

		[Test]
		public void Return_Empty_Enumerable()
		{
			//Arrange
			var repo = new Mock<IFeedsRepository>();
			repo.Setup(x => x.Find(It.IsAny<FeedFilter>()))
				.Returns((IEnumerable<Feed>)null);
			var uk = new FindFeed(repo.Object);

			//Act
			var feeds = uk.Find(new FeedFilter());

			//Assert
			feeds.Should().NotBeNull();
			feeds.Count().Should().Be(0);
		}

		[Test]
		public void Success_Find_Feeds()
		{
			//Arrange
			var feeds = new List<Feed>
			{
				Feed.GetBuilder().Id(1).OriginalUrl(new Uri("http://feed.com/1")).Build(),
				Feed.GetBuilder().Id(2).OriginalUrl(new Uri("http://feed.com/2")).Build()
			};
			var repo = new Mock<IFeedsRepository>();
			repo.Setup(x => x.Find(It.IsAny<FeedFilter>()))
				.Returns(feeds);
			var uk = new FindFeed(repo.Object);

			//Act
			var result = uk.Find(new FeedFilter());

			//Assert
			result.Should().NotBeNull();
			result.Count().Should().Be(2);
		}
	}
}