using System;
using FluentAssertions;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Enums;
using Mindscan.Media.Domain.Exceptions;
using Mindscan.Media.UseCase.Ports;
using Mindscan.Media.UseCase.Scheduler.Feeds;
using Moq;
using NUnit.Framework;

namespace Mindscan.Media.UseCase.UnitTests.Scheduler.Feeds
{
	[TestFixture]
	public class UpdateFeed_Tests
	{

		[Test]
		public void Throw_If_Feed_Empty()
		{
			//Arrange
			var uk = new UpdateFeed(null);

			//Act
			var error1 = Assert.Throws<ArgumentNullException>(() => uk.Update(null));
			var error2 = Assert.Throws<ArgumentNullException>(() => uk.Update(null, null));


			//Assert
			error1.Should().NotBeNull();
			error1.Message.Should().Be("Value cannot be null.\r\nParameter name: feed");
			error2.Should().NotBeNull();
			error2.Message.Should().Be("Value cannot be null.\r\nParameter name: feed");
		}

		[Test]
		public void Throw_If_ActualUrl_Empty()
		{
			//Arrange
			var uk = new UpdateFeed(null);
			var feed = GetBuilder().Build();

			//Act
			var error = Assert.Throws<ArgumentNullException>(() => uk.Update(feed, null));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: actualUrl");
		}

		[Test]
		public void Throw_If_Old_Not_Found()
		{
			//Arrange
			var repo = new Mock<IFeedsRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns((Feed)null);
			var uk = new UpdateFeed(repo.Object);
			var feed = GetBuilder().Build();
			var newUrl = NormalizedUrl.Build(new Uri("https://feed.url"));

			//Act
			var error1 = Assert.Throws<EntityNotFoundException>(() => uk.Update(feed));
			var error2 = Assert.Throws<EntityNotFoundException>(() => uk.Update(feed, newUrl));

			//Assert
			error1.Should().NotBeNull();
			error1.Message.Should().Be("Feed with id=1 not found.");
			error2.Should().NotBeNull();
			error2.Message.Should().Be("Feed with id=1 not found.");
		}

		[Test]
		public void Throw_If_Outdated()
		{
			//Arrange
			var now = DateTime.UtcNow;
			var oldFeed = GetBuilder().UpdatedAtUtc(now.AddSeconds(-2)).Build();
			var repo = new Mock<IFeedsRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(oldFeed);
			var uk = new UpdateFeed(repo.Object);
			var feed = GetBuilder().UpdatedAtUtc(now).Build();

			//Act
			var error1 = Assert.Throws<EntityOutdatedException>(() => uk.Update(feed));
			var error2 = Assert.Throws<EntityOutdatedException>(() =>
				uk.Update(
					feed,
					NormalizedUrl.Build(new Uri("http://www.feed.url"))
				));

			//Assert
			error1.Should().NotBeNull();
			error1.Message.Should().Be("Entity 'Feed' with id=1 is outdated.");
			error2.Should().NotBeNull();
			error2.Message.Should().Be("Entity 'Feed' with id=1 is outdated.");
		}

		[Test]
		public void Throw_If_No_Changes()
		{
			//Arrange
			var feed = GetBuilder()
				.OriginalUrl(new Uri("http://original.uri"))
				.ActualUrl(new Uri("http://actual.url"))
				.Type(FeedType.Custom)
				.Encoding("test")
				.Description("description")
				.AdditionalInfo("additional info")
				.Build();
			var repo = new Mock<IFeedsRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(feed);
			var uk = new UpdateFeed(repo.Object);

			//Act
			var error = Assert.Throws<EntityHasNoChangesException>(() => uk.Update(feed));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Entity Feed with id=1 has no changes.");
		}

		[Test]
		public void Throw_If_Repository_Returned_Empty()
		{
			//Arrange
			var now = DateTime.UtcNow;
			var oldFeed = GetBuilder().UpdatedAtUtc(now).Build();
			var repo = new Mock<IFeedsRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(oldFeed);
			var newFeed = GetBuilder().UpdatedAtUtc(now).Type(FeedType.Rss).Build();
			repo.Setup(x => x.Update(It.IsAny<Feed>()))
				.Returns((Feed)null);
			var actualUrl = NormalizedUrl.Build(new Uri("http://test.uri"));
			var uk = new UpdateFeed(repo.Object);

			//Act
			var error = Assert.Throws<EntityFailToUpdateException>(() => uk.Update(newFeed));
			var error2 = Assert.Throws<EntityFailToUpdateException>(() => uk.Update(oldFeed, actualUrl));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Entity 'Feed' with id=1 was not updated.");
			error2.Should().NotBeNull();
			error2.Message.Should().Be("Entity 'Feed' with id=1 was not updated.");
		}

		[TestCase("http://original.uri", "https://original.uri")]
		[TestCase("http://original.uri", "http://www.original.uri")]
		[TestCase("http://original.uri", "https://www.original.uri")]
		[TestCase("http://original.uri", "http://original.uri/1")]
		public void Success_Update_Original_Url(string oldUrl, string newUrl)
		{
			//Arrange
			var now = DateTime.UtcNow;
			var oldFeed = GetBuilder().OriginalUrl(new Uri(oldUrl)).UpdatedAtUtc(now).Build();
			var repo = new Mock<IFeedsRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(oldFeed);
			var newFeed = GetBuilder().OriginalUrl(new Uri(newUrl)).UpdatedAtUtc(now).Build();
			repo.Setup(x => x.Update(It.IsAny<Feed>()))
				.Returns(newFeed);
			var uk = new UpdateFeed(repo.Object);

			//Act
			var result = uk.Update(newFeed);
			var nUrl = NormalizedUrl.Build(new Uri(newUrl));

			//Assert
			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(newFeed);
			result.OriginalUrl.Should().Be(nUrl);
		}

		[TestCase("http://actual.uri", "https://actual.uri")]
		[TestCase("http://actual.uri", "http://www.actual.uri")]
		[TestCase("http://actual.uri", "https://www.actual.uri")]
		[TestCase("http://actual.uri", "http://actual.uri/1")]
		public void Success_Update_Actual_Url(string oldUrl, string newUrl)
		{
			//Arrange
			var originalUrl = new Uri("http://original.uri");
			var now = DateTime.UtcNow;
			var oldFeed = GetBuilder()
				.OriginalUrl(originalUrl)
				.ActualUrl(new Uri(oldUrl))
				.UpdatedAtUtc(now).Build();
			var repo = new Mock<IFeedsRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(oldFeed);

			var newFeed = GetBuilder()
				.OriginalUrl(originalUrl)
				.ActualUrl(new Uri(newUrl))
				.UpdatedAtUtc(now).Build();
			repo.Setup(x => x.Update(It.IsAny<Feed>()))
				.Returns(newFeed);
			repo.Setup(x => x.UpdateActualUrl(It.IsAny<Feed>()))
				.Returns((Feed x) => x);
			var uk = new UpdateFeed(repo.Object);

			//Act
			var result = uk.Update(newFeed);
			var nUrl = NormalizedUrl.Build(new Uri(newUrl));
			var result2 = uk.Update(oldFeed, nUrl);
			

			//Assert
			result.Should().NotBeNull();
			result.Should().BeEquivalentTo(newFeed);
			result.ActualUrl.Should().Be(nUrl);

			result2.Should().NotBeNull();
			result2.Should().BeEquivalentTo(oldFeed);
			result2.ActualUrl.Should().Be(nUrl);
		}

		[Test]
		public void Success_Update_Rest_Fields()
		{
			//Arrange
			var originalUrl = new Uri("http://original.uri");
			var now = DateTime.UtcNow;
			var oldFeed = GetBuilder()
				.OriginalUrl(originalUrl)
				.UpdatedAtUtc(now)
				.Type(FeedType.Html)
				.AdditionalInfo("AdditionalInfo")
				.Description("Description")
				.Encoding("utf-16")
				.Build();
			var repo = new Mock<IFeedsRepository>();
			repo.Setup(x => x.Find(It.IsAny<long>()))
				.Returns(oldFeed);
			var newFeed = GetBuilder()
				.OriginalUrl(originalUrl)
				.UpdatedAtUtc(now)
				.Type(FeedType.Custom)
				.AdditionalInfo("Additional")
				.Description("Desc")
				.Encoding("win-1251")
				.Build();
			repo.Setup(x => x.Update(It.IsAny<Feed>()))
				.Returns(newFeed);
			var uk = new UpdateFeed(repo.Object);

			//Act
			var result = uk.Update(newFeed);

			//Assert
			result.Should().NotBeNull();
			result.Should().NotBeEquivalentTo(oldFeed);
			result.Should().BeEquivalentTo(newFeed);
		}

		private static FeedBuilder GetBuilder()
		{
			return Feed
				.GetBuilder()
				.Id(1)
				.OriginalUrl(new Uri("http://feed.com"))
				.CreatedAtUtc(DateTime.UtcNow)
				.UpdatedAtUtc(DateTime.UtcNow);
		}
	}
}