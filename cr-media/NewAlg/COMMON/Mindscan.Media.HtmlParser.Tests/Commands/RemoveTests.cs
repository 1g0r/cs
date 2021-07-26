using System;
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
	internal class RemoveTests : ParserTestsBase
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
			var command = SupportedCommands.Remove();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("<div><p class='one'></p><p class='two'></p> <p>Waffle</p></div>", "p", 1, ".one", ".two")]
		[TestCase("<div><p class='one'></p><p class='two'></p></div>", "p", 0, ".one", ".two")]
		[TestCase("<div><p class='one'></p><p class='two'></p></div>", "p", 2, "script")]
		[TestCase("<div><script></script><p class='one'></p><p class='two'></p></div>", "script", 0, "script")]
		public void TakesIParentNode(string htmlBody, string selector, int resultCount, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.Remove(patterns);
			var doc = GetDocument(htmlBody);

			//Act 
			var result = (IDocument)command.Run(GetContext(), doc);
			var count = result.QuerySelectorAll(selector).Length;

			//Assert
			result.Should().NotBeNull();
			count.Should().Be(resultCount);
		}

		[TestCase("<div><p class='one'></p><p class='two'></p> <p>Waffle</p></div>", "p", 1, "*[@class='one']", "*[@class='two']")]
		[TestCase("<div><p class='one'></p><p class='two'></p></div>", "p", 0, "*[@class='one']", "*[@class='two']")]
		[TestCase("<div><p class='one'></p><p class='two'></p></div>", "p", 2, "script")]
		[TestCase("<div><script></script><p class='one'></p><p class='two'></p></div>", "script", 0, "script")]
		public void TakesXElement(string html, string xPath, int resultCount, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.Remove(patterns);
			var element = (SupportedCommands.XPath().Run(GetContext(), html) as XDocument)?.Root;

			//Act
			var result = (XNode)command.Run(GetContext(), element);
			var count = element.XPathSelectElements(xPath).Count();

			//Assert
			result.Should().NotBeNull();
			count.Should().Be(resultCount);
		}
	}
}