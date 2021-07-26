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
	internal class SkipTests : ParserTestsBase
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
			var command = SupportedCommands.Skip();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		static object[] _stringsCases = {
			new object[] { new [] {"Waffle", "foo"}, new [] {"waffle", "FOO"}, 0 },
			new object[] { new [] {"Waffle", "foo"}, null, 2 },
			new object[] { new [] {"mandalay", "Waffle", "foo"}, new [] {"mandalay"}, 2 },
			new object[] { new [] {"mandalay", "Waffle", "foo", "", " "}, new [] {"mandalay"}, 3 },
			new object[] { new [] {"Кто такой Пушкин?", "Waffle", "foo", "", "Пушкин - великий поэт"}, new [] {"mandalay", "^пушкин"}, 3 }
		};
		[TestCaseSource(nameof(_stringsCases))]
		public void CollectionOfStrings(string[] values, string[] patterns, int resultCount)
		{
			//Arrange
			var command = SupportedCommands.Skip(patterns);

			//Act
			var result = (command.Run(GetContext(), values) as IEnumerable<string>)?.ToList();
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
			}
			
		}

		[TestCase("<div></div>", 0, ".gotcha", "img")]
		[TestCase("<div> <p class='gotcha'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 3, ".gotcha")]
		[TestCase("<div> <p class='gotcha'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 2, ".gotcha", "img")]
		[TestCase("<div> <p class='gotcha'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img/></div>", 0, "p", "img")]
		public void CollectionOfIElements(string htmlBody, int resultCount, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.Skip(patterns);
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
			}
		}

		[TestCase("<div></div>", 0, "*[@class='gotcha']", "img")]
		[TestCase("<div> <p class='gotcha'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img></img></div>", 3, "*[@class='gotcha']")]
		[TestCase("<div> <p class='gotcha'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img></img></div>", 2, "*[@class='gotcha']", "img")]
		[TestCase("<div> <p class='gotcha'></p>  <p></p>  <p></p>  <p class='gotcha'></p> <img></img></div>", 0, "p", "img")]
		public void CollectionOfXElements(string html, int resultCount, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.Skip(patterns);
			var elements = (SupportedCommands.XPath().Run(GetContext(), html) as XDocument)?.XPathSelectElements("//div/*").ToList();

			//Act
			var resultEnum = command.Run(GetContext(), elements) as IEnumerable<XElement> ?? Enumerable.Empty<XElement>();
			var result = resultEnum.ToArray();

			//Assert
			result.Should().NotBeNull();
			result.Length.Should().Be(resultCount);
		}
	}
}