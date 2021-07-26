using System;
using System.IO;
using FluentAssertions;
using Mindscan.Media.Domain.Const;
using Mindscan.Media.Domain.Entities.Scheduler;
using Mindscan.Media.Domain.Exceptions;
using NUnit.Framework;

namespace Mindscan.Media.Domain.Tests.Entities
{
	[TestFixture]
	public class FeedBuilder_Tests
	{
		[Test]
		public void Throw_If_Call_Twice()
		{
			//Arrange
			var builder = Feed.GetBuilder().OriginalUrl(new Uri("http://feed.uri"));
			var feed = builder.Build();

			//Act
			var error = Assert.Throws<InvalidOperationException>(() => builder.Build());

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Build method can be called only ones.");
		}

		[Test]
		public void OriginalUrl_Required()
		{
			//Arrange
			var builder = Feed.GetBuilder();

			//Act
			var error = Assert.Throws<RequiredFieldException>(() => builder.Build());

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: OriginalUrl");
		}

		[Test]
		public void CreatedAtUtc_ShouldBe_Utc()
		{
			//Arrange
			var builder = Feed.GetBuilder()
				.OriginalUrl(new Uri("http://waffle.foo"));

			//Act
			var error = Assert.Throws<NotUtcTimeZoneException>(() => builder.CreatedAtUtc(DateTime.Now));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("DateTime should be in UTC time zone.\r\nParameter name: CreatedAtUtc");
		}

		[Test]
		public void UpdatedAtUtc_ShouldBe_Utc()
		{
			//Arrange
			var builder = Feed.GetBuilder()
				.OriginalUrl(new Uri("http://waffle.foo"));

			//Act
			var error = Assert.Throws<NotUtcTimeZoneException>(() => builder.UpdatedAtUtc(DateTime.Now));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("DateTime should be in UTC time zone.\r\nParameter name: UpdatedAtUtc");
		}

		[Test]
		public void ActualUrl_Should_Differ()
		{
			//Arrange
			var url = new Uri("http://waffle.net");
			var builder = Feed.GetBuilder()
				.OriginalUrl(url)
				.ActualUrl(url);

			//Act 
			var error = Assert.Throws<InvalidDataException>(() => builder.Build());

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("ActualUrl should differ from OriginalUrl.");
		}

		[Test]
		public void ActualUrl_ShouldNot_Differ()
		{
			//Arrange
			var builder = Feed.GetBuilder()
				.OriginalUrl(new Uri("http://waffle.net"))
				.ActualUrl(new Uri("https://waffle.net"));

			//Act 
			var feed = builder.Build();

			//Assert
			feed.Should().NotBeNull();
			feed.OriginalUrl.Should().NotBeNull();
			feed.OriginalUrl.Prefix.Should().Be("http://");
			feed.ActualUrl.Should().NotBeNull();
			feed.ActualUrl.Prefix.Should().Be("https://");
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase("  ")]
		public void Encoding_ShouldBe_Default(string value)
		{
			//Arrange
			var builder = Feed.GetBuilder()
				.OriginalUrl(new Uri("http://waffle.net"))
				.Encoding(value);

			//Act
			var feed = builder.Build();

			//Assert
			feed.Should().NotBeNull();
			feed.Encoding.Should().Be(GlobalDefaults.Encoding);
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase("  ")]
		public void Description_ShouldBe_Default(string value)
		{
			//Arrange
			var builder = Feed.GetBuilder()
				.OriginalUrl(new Uri("http://waffle.net"))
				.Description(value);

			//Act
			var feed = builder.Build();

			//Assert
			feed.Should().NotBeNull();
			feed.Description.Should().BeNull();
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase("  ")]
		public void AdditionalInfo_ShouldBe_Default(string value)
		{
			//Arrange
			var builder = Feed.GetBuilder()
				.OriginalUrl(new Uri("http://waffle.net"))
				.AdditionalInfo(value);

			//Act
			var feed = builder.Build();

			//Assert
			feed.Should().NotBeNull();
			feed.AdditionalInfo.Should().BeNull();
		}
	}

	
}