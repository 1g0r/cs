using System;
using System.Xml.Linq;
using System.Xml.XPath;
using AngleSharp.Dom;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	class NextTests : ParserTestsBase
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
			var command = SupportedCommands.Next();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase(null, true, ".gotcha")]
		[TestCase("", true, ".gotcha")]
		[TestCase("<div><p /><p /><p class='gotcha' /><p /></div>", true, ".gotcha")]
		[TestCase("<div><p class='brother'/><p /><p /><p class='gotchaa'/></div>", true, ".gotcha")]
		[TestCase("<div><p /><p /><p class='gotcha' /><p class='brother'/></div>", true, ".gotcha")]
		[TestCase("<div><p /><p class='brother'/><p class='gotcha'>waffle</p><p /></div>", false, ".gotcha")]
		[TestCase("<div><p class='brother'/><p /><p /><p class='gotcha'>waffle</p></div>", false, ".gotcha")]
		[TestCase("<div><p class='brother'/><p /><p /><p class='gotcha'>waffle</p></div>", false, "a", ".gotcha")]
		public void TakeIElement(string html, bool isNull, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.Next(patterns);
			var element = GetElement(html, ".brother");

			//Act
			var result = command.Run(GetContext(), element) as IElement;

			//Assert
			if (isNull)
				result.Should().BeNull();
			else
			{
				result.Should().NotBeNull();
				result.TextContent.Should().BeEquivalentTo("waffle");
			}
		}

		[TestCase("<div />", true, "*[@class='gotcha']")]
		[TestCase("<div />", true, "*[@class='gotcha']")]
		[TestCase("<div><p /><p /><p class='gotcha' /><p /></div>", true, "*[@class='gotcha']")]
		[TestCase("<div><p class='brother'/><p /><p /><p class='gotchaa'/></div>", true, "*[@class='gotcha']")]
		[TestCase("<div><p /><p /><p class='gotcha' /><p class='brother'/></div>", true, "*[@class='gotcha']")]
		[TestCase("<div><p /><p class='brother'/><p class='gotcha'>waffle</p><p /></div>", false, "*[@class='gotcha']")]
		[TestCase("<div><p class='brother'/><p /><p /><p class='gotcha'>waffle</p></div>", false, "*[@class='gotcha']")]
		[TestCase("<div><p class='brother'/><p /><p /><p class='gotcha'>waffle</p></div>", false, "a", "*[@class='gotcha']")]
		public void TakeXElement(string html, bool isNull, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.Next(patterns);
			var doc = (XDocument)SupportedCommands.XPath().Run(GetContext(), html);
			var element = doc.XPathSelectElement("//*[@class='brother']");

			//Act
			var result = command.Run(GetContext(), element) as XElement;

			//Assert
			if (isNull)
				result.Should().BeNull();
			else
			{
				result.Should().NotBeNull();
				result.Value.Should().BeEquivalentTo("waffle");
			}
		}

		private IElement GetElement(string html, string selector)
		{
			var doc = GetDocument(html);
			return doc.QuerySelector(selector);
		}
	}
}