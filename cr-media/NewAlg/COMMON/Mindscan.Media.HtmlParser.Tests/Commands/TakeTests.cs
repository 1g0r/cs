using AngleSharp.Dom;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	internal class TakeTests : ParserTestsBase
	{
		private static object[] _cases =
		{
			666,
			DateTimeOffset.Now,
			null,
			typeof(AttrTests),
			new[] { 1, 2, 3 }
		};
		[TestCaseSource(nameof(_cases))]
		public void NotSupportedContextType(object value)
		{
			//Arrange
			var command = SupportedCommands.Take();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("<div></div>", 0, 1)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 1, -1)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 1, 0)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 2, 2)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 3, 3)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 5, 6)]
		public void CollectionOfIElements(string htmlBody, int resultCount, int takeCount)
		{
			//Arrange
			var command = SupportedCommands.Take(takeCount);
			var elements = GetDocument(htmlBody).QuerySelectorAll("p, img");

			//Act
			var result = (command.Run(GetContext(), elements) as IEnumerable<IElement>)?.ToList();
			var count = result?.Count;

			//Assert
			if (resultCount == 0)
			{
				result.Should().BeNull();
			}
			else
			{
				result.Should().NotBeNull();
				count.Should().Be(resultCount);
				if (resultCount > 0)
				{
					result[0].ClassName.Should().BeEquivalentTo("first");
				}
			}
		}

		[TestCase("<div></div>", 0, 1)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 1, -1)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 1, 0)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 1, 1)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 2, 2)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 4, 5)]
		public void CollectionOfXElements(string html, int resultCount, int takeCount)
		{
			//Arrange
			var command = SupportedCommands.Take(takeCount);
			var elements = (SupportedCommands.XPath().Run(GetContext(), html) as XDocument)?.XPathSelectElements("//p").ToList();

			//Act
			var resultEnum = command.Run(GetContext(), elements) as IEnumerable<XElement> ?? Enumerable.Empty<XElement>();
			var result = resultEnum.ToArray();

			//Assert
			result.Should().NotBeNull();
			result.Length.Should().Be(resultCount);
			if (resultCount > 0)
			{
				result[0].Attribute("class").Value.Should().BeEquivalentTo("first");
			}
		}

		[TestCase(new string[] { "1", "2", "3", "4" }, 1, 1)]
		[TestCase(new string[] { "1", "2", "3", "4" }, 1, -1)]
		[TestCase(new string[] { "1", "2", "3", "4" }, 1, 0)]
		[TestCase(new string[] { "1", "2", "3", "4" }, 1, 1)]
		[TestCase(new string[] { "1", "2", "3", "4" }, 4, 5)]
		public void CollectionOfStrings(string[] strings, int resultCount, int takeCount)
		{
			//Arrange
			var command = SupportedCommands.Take(takeCount);
			var elements = strings.ToList();

			//Act
			var resultEnum = command.Run(GetContext(), elements) as IEnumerable<string> ?? Enumerable.Empty<string>();
			var result = resultEnum.ToArray();

			//Assert
			result.Should().NotBeNull();
			result.Length.Should().Be(resultCount);
			if (resultCount > 0)
			{
				result[0].Should().BeEquivalentTo("1");
			}
		}

		[TestCase("{\"a\": \"1\",\"b\": \"2\",\"c\": \"3\",\"d\": \"4\"}", 1, 1)]
		[TestCase("{\"a\": \"1\",\"b\": \"2\",\"c\": \"3\",\"d\": \"4\"}", 1, -1)]
		[TestCase("{\"a\": \"1\",\"b\": \"2\",\"c\": \"3\",\"d\": \"4\"}", 1, 0)]
		[TestCase("{\"a\": \"1\",\"b\": \"2\",\"c\": \"3\",\"d\": \"4\"}", 1, 1)]
		[TestCase("{\"a\": \"1\",\"b\": \"2\",\"c\": \"3\",\"d\": \"4\"}", 4, 5)]
		public void CollectionOfJTokens(string jTokens, int resultCount, int takeCount)
		{
			//Arrange
			var command = SupportedCommands.Take(takeCount);
			var elements = JToken.Parse(jTokens).SelectTokens("$.*");

			//Act
			var resultEnum = command.Run(GetContext(), elements) as IEnumerable<JToken> ?? Enumerable.Empty<JToken>();
			var result = resultEnum.ToArray();

			//Assert
			result.Should().NotBeNull();
			result.Length.Should().Be(resultCount);
			if (resultCount > 0)
			{
				result[0].Value<string>().Should().BeEquivalentTo("1");
			}
		}
	}
}