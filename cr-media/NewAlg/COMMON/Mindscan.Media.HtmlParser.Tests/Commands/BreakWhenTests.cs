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
	class BreakWhenTests : ParserTestsBase
	{
		private static object[] _cases =
		{
			"",
			666,
			DateTimeOffset.Now,
			new[] { 1, 2, 3 },
			typeof(AttrTests),
			null
		};
		[TestCaseSource(nameof(_cases))]
		public void NotSupportedContextType(object value)
		{
			//Arrange
			var command = SupportedCommands.BreakWhen();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		
		[TestCase(3, new[] { "santa", "radio" }, new[] { "foo", "shikaka", "waffle" })]
		[TestCase(0, new[] { "santa", "radio" }, new string[0])]
		[TestCase(3, null, new[] { "foo", "shikaka", "waffle" })]
		[TestCase(3, new string[0], new[] { "foo", "shikaka", "waffle" })]
		[TestCase(1, new[] { "santa", "^radio\\s+" }, new[] { "foo radio", "radio shikaka", "radio waffle" })]
		public void CollectionOfStrings(int resultCount, string[] patterns, string[] values)
		{
			//Arrange
			var command = SupportedCommands.BreakWhen(patterns);

			//Act
			var resultEnum = command.Run(GetContext(), values) as IEnumerable<string>;
			var result = resultEnum?.ToArray();


			//Assert
			if (patterns == null || patterns.Length == 0 || resultCount == 0)
			{
				result.Should().BeNull();
			}
			else
			{
				result.Should().NotBeNull();
				result.Length.Should().Be(resultCount);
			}
		}

		[TestCase("<div><p>one</p><p>two</p><p>three</p><p class='footer'>four</p></div>", 4)]
		[TestCase("<div><p>one</p><p>two</p><p>three</p><p class='footer'>four</p></div>", 4, new string[0])]
		[TestCase("<div><p>one</p><p>two</p><p>three</p><p class='footer'>four</p></div>", 4, new[] { "table", ".header" })]
		[TestCase(null, 0, new[] { "table", ".header" })]
		[TestCase("<div><p>one</p><p>two</p><p>three</p><p class='footer'>four</p></div>", 0, new[] { "div > p", ".footer" })]
		[TestCase("<div><p>one</p><p>two</p><p>three</p><p class='footer'>four</p></div>", 3, new[] { "div > p:has(img)", ".footer" })]
		public void CollectionOfIElement(string elementsHtml, int resultCount, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.BreakWhen(patterns);
			var elements = GetElements(elementsHtml, "div > p");

			//Act
			var resultEnum = command.Run(GetContext(), elements) as IEnumerable<IElement>;
			var result = resultEnum?.ToArray();

			//Assert
			if (patterns == null || patterns.Length == 0 || resultCount == 0)
			{
				result.Should().BeNull();
			}
			else
			{
				result.Should().NotBeNull();
				result.Length.Should().Be(resultCount);
			}
		}

		[TestCase("<div><p>one</p><p>two</p><p>three</p><p class='footer'>four</p></div>", 4)]
		[TestCase("<div><p>one</p><p>two</p><p>three</p><p class='footer'>four</p></div>", 4, new string[0])]
		[TestCase("<div><p>one</p><p>two</p><p>three</p><p class='footer'>four</p></div>", 4, new[] { "table", "*[@class='header']" })]
		[TestCase("<div></div>", 0, new[] { "table", ".header" })]
		[TestCase("<div><p>one</p><p>two</p><p>three</p><p class='footer'>four</p></div>", 0, new[] { "p", "*[@class='footer']" })]
		[TestCase("<div><p>one</p><p>two</p><p>three</p><p class='footer'>four</p></div>", 3, new[] { "p[./img]", "*[@class='footer']" })]
		public void CollectionOfXElement(string html, int resultCount, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.BreakWhen(patterns);
			var doc = (XDocument)SupportedCommands.XPath().Run(GetContext(), html);
			var elements = doc.XPathSelectElements("//p");

			//Act
			var resultEnum = command.Run(GetContext(), elements) as IEnumerable<XElement>;
			var result = resultEnum?.ToArray();

			//Assert
			if (patterns == null || patterns.Length == 0 || resultCount == 0)
			{
				result.Should().BeNull();
			}
			else
			{
				result.Should().NotBeNull();
				result.Length.Should().Be(resultCount);
			}
		}

		private IEnumerable<IElement> GetElements(string html, string selector)
		{
			var doc = GetDocument(html);
			return doc.QuerySelectorAll(selector);
		}
	}
}