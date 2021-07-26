using System;
using FluentAssertions;
using Mindscan.Media.Domain.Entities.Collector;
using Mindscan.Media.Domain.Exceptions;
using NUnit.Framework;

namespace Mindscan.Media.Domain.Tests.Entities
{
	[TestFixture]
	public class MaterialBuilder_Tests
	{
		[Test]
		public void Throw_If_Call_Twice()
		{
			//Arrange
			var builder = Material.GetBuilder()
				.SourceId(1)
				.OriginalUrl(new Uri("http://original.uri"))
				.ActualUrl(new Uri("http://actual.com"))
				.Title("title")
				.Text("text")
				.Host(new Uri("http://host.ru"))
				.PublishedAtUtc(DateTime.UtcNow);
			var material = builder.Build();

			//Act
			var error = Assert.Throws<InvalidOperationException>(() => builder.Build());

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Build method can be called only ones.");
		}

		[Test]
		public void SourceId_Required()
		{
			//Arrange
			var builder = Material.GetBuilder();

			//Act
			var error = Assert.Throws<RequiredFieldException>(() => builder.Build());

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: SourceId");
		}

		[Test]
		public void OriginalUrl_Required()
		{
			//Arrange
			var builder = Material.GetBuilder()
				.SourceId(666);

			//Act
			var error = Assert.Throws<RequiredFieldException>(() => builder.Build());

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: OriginalUrl");
		}

		[Test]
		public void ActualUri_Required()
		{
			//Arrange
			var builder = Material.GetBuilder()
				.OriginalUrl(new Uri("http://waffle.com/pages/123456.html"))
				.SourceId(666);

			//Act
			var error = Assert.Throws<RequiredFieldException>(() => builder.Build());

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: ActualUrl");
		}

		[Test]
		public void Title_Required()
		{
			//Arrange
			var url = new Uri("http://waffle.com/pages/123456.html");
			var builder = Material.GetBuilder()
				.OriginalUrl(url)
				.ActualUrl(url)
				.SourceId(666);

			//Act
			var error = Assert.Throws<RequiredFieldException>(() => builder.Build());

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: Title");
		}

		[Test]
		public void Text_Required()
		{
			//Arrange
			var url = new Uri("http://waffle.com/pages/123456.html");
			var builder = Material.GetBuilder()
				.OriginalUrl(url)
				.ActualUrl(url)
				.Title("Title")
				.SourceId(666);

			//Act
			var error = Assert.Throws<RequiredFieldException>(() => builder.Build());

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: Text");
		}

		[Test]
		public void Host_Required()
		{
			//Arrange
			var url = new Uri("http://waffle.com/pages/123456.html");
			var builder = Material.GetBuilder()
				.OriginalUrl(url)
				.ActualUrl(url)
				.Title("Title")
				.Text("Text")
				.SourceId(666);

			//Act
			var error = Assert.Throws<RequiredFieldException>(() => builder.Build());

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: Host");
		}

		[Test]
		public void PublishedAtUtc_Required()
		{
			//Arrange
			var url = new Uri("http://waffle.com/pages/123456.html");
			var builder = Material.GetBuilder()
				.OriginalUrl(url)
				.ActualUrl(url)
				.Title("Title")
				.Text("Text")
				.Host(new Uri("http://host.com"))
				.SourceId(666);

			//Act
			var error = Assert.Throws<RequiredFieldException>(() => builder.Build());

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("Value cannot be null.\r\nParameter name: PublishedAtUtc");
		}

		[Test]
		public void PublishedAtUtc_ShouldBe_Utc()
		{
			//Arrange
			var url = new Uri("http://waffle.com/pages/123456.html");
			var builder = Material.GetBuilder()
				.OriginalUrl(url)
				.ActualUrl(url)
				.Title("Title")
				.Text("Text")
				.Host(new Uri("http://host.com"));

			//Act
			var error = Assert.Throws<NotUtcTimeZoneException>(() => builder.PublishedAtUtc(DateTime.Now));

			//Assert
			error.Should().NotBeNull();
			error.Message.Should().Be("DateTime should be in UTC time zone.\r\nParameter name: PublishedAtUtc");
		}
	}
}