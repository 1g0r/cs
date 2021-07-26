using System;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	class ToUrlTests : ParserTestsBase
	{
		private static object[] _cases =
		{
			"",
			666,
			DateTimeOffset.Now,
			null,
			typeof(AttrTests)
		};

		[TestCaseSource(nameof(_cases))]
		public void NotSupportedContextType(object value)
		{
			//Arrange
			var command = SupportedCommands.ToUrl();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("http://site.ru/waffle", "//site.ru/image.jpg", "http://site.ru/image.jpg")]
		[TestCase("http://site.ru/waffle/", "//site.ru/image.jpg", "http://site.ru/image.jpg")]
		[TestCase("https://site.ru/waffle/", "//site.ru/image.jpg", "https://site.ru/image.jpg")]
		[TestCase("http://site.ru/waffle/", "/image.jpg", "http://site.ru/image.jpg")]
		[TestCase("http://site.ru/waffle", "/image.jpg", "http://site.ru/image.jpg")]
		[TestCase("http://site.ru/waffle", "image.jpg", "http://site.ru/waffle/image.jpg")]
		[TestCase("http://site.ru/waffle/", "image.jpg", "http://site.ru/waffle/image.jpg")]
		[TestCase("http://site.ru/waffle/", "http://image.jpg", "http://image.jpg")]
		[TestCase("http://site.ru/waffle/", "https://image.jpg", "https://image.jpg")]
		[TestCase("http://www.altai_rep.izbirkom.ru/news/", "/news/?ELEMENT_ID=1313", "http://www.altai_rep.izbirkom.ru/news/?ELEMENT_ID=1313")]
		[TestCase("http://www.altai_rep.izbirkom.ru/news/?ELEMENT_ID=1320", "/news/?ELEMENT_ID=1313", "http://www.altai_rep.izbirkom.ru/news/?ELEMENT_ID=1313")]
		[TestCase("https://www.altai_rep.izbirkom.ru:8080/news/?ELEMENT_ID=1320", "/news/?ELEMENT_ID=1320", "https://www.altai_rep.izbirkom.ru:8080/news/?ELEMENT_ID=1320")]
		public void TakesPattern(string url, string param, string outcome)
		{
			//Arrange
			var command = SupportedCommands.ToUrl();

			//Act
			var result = (Uri)command.Run(GetContext(url), param);

			//Assert
			result.Should().BeEquivalentTo(new Uri(outcome));
		}

		[TestCase("http://site.ru", "data:image/sdflskjdf", null)]
		[TestCase("http://site.ru", "denied:data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD", null)]
		[TestCase("http://site.ru", "file:///C:Users1EF9~1AppDataLocalTempmsohtmlclip11clip_image001.gif", null)]
		public void NotAccept(string url, string param, string outcome)
		{
			//Arrange
			var command = SupportedCommands.ToUrl();

			//Act
			var result = (Uri)command.Run(GetContext(url), param);

			//Assert
			result.Should().BeNull();
		}
	}
}