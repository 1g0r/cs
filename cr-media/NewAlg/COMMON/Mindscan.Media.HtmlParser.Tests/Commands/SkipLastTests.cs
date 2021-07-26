using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using AngleSharp.Dom;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	internal class SkipLastTests : ParserTestsBase
	{
		private static object[] _cases =
		{
			"",
			"waffle",
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
			var command = SupportedCommands.SkipLast();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("<div></div>", 0, 1)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 4, -1)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 4, 0)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 4, 1)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 2, 3)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 0, 5)]
		public void CollectionOfIElements(string htmlBody, int resultCount, int skipCount)
		{
			//Arrange
			var command = SupportedCommands.SkipLast(skipCount);
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
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 3, -1)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 3, 0)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 3, 1)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 2, 2)]
		[TestCase("<div> <p class='first'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 0, 5)]
		public void CollectionOfXElements(string html, int resultCount, int skipCount)
		{
			//Arrange
			var command = SupportedCommands.SkipLast(skipCount);
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
	}
}