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
	internal class TakeFirstTests : ParserTestsBase
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
			var command = SupportedCommands.TakeFirst();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		private static object[] _stringCases =
		{
			new object[] { new [] {null, "", null}, new [] { "" }, null },
			new object[] { new [] { "Waffle", "FOO", "shikaka" }, new [] {"^mirror"}, null },
			new object[] { new [] { "Waffle", "FOO", "shikaka" }, new [] {"^foo"}, "FOO" },
			new object[] { new [] { "", null, "FOO", "shikaka" }, new [] {"^foo"}, "FOO" }
		};
		[TestCaseSource(nameof(_stringCases))]
		public void CollectionOfStrings(string[] values, string[] patterns, string outcome)
		{
			//Arrange
			var command = SupportedCommands.TakeFirst(patterns);

			//Act
			var result = (string)command.Run(GetContext(), values);

			//Assert
			result.Should().BeEquivalentTo(outcome);
		}

		[TestCase("<div> <p></p> <p>waffle</p> <p></p> </div>", null, ".gotcha")]
		[TestCase("<div> <p></p> <p class='gotcha'>waffle</p> <p class='gotcha'>foo</p> </div>", "waffle", ".gotcha", ".gotcha")]
		[TestCase("<div> <p>waffle</p> <p class='gotcha'>waffle</p> <p class='gotcha'>foo</p> </div>", "waffle")]
		[TestCase("<div> <p></p> <img></img> <p class='gotcha'>waffle</p> <p class='gotcha'>foo</p> </div>", "", ".gotcha", "img")]
		public void CollectionOfIElements(string htmlBody, string outcome, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.TakeFirst(patterns);
			var elements = GetDocument(htmlBody).QuerySelectorAll("p, img");

			//Act
			var result = (IElement)command.Run(GetContext(), elements);

			//Assert
			if (outcome == null)
			{
				result.Should().BeNull();
			}
			else
			{
				result.Should().NotBeNull();
				result.TextContent.Should().BeEquivalentTo(outcome);
			}
		}

		[TestCase("<div> <p></p> <p>waffle</p> <p></p> </div>", null, "*[@class='gotcha']")]
		[TestCase("<div> <p></p> <p class='gotcha'>waffle</p> <p class='gotcha'>foo</p> </div>", "waffle", "*[@class='gotcha']", "*[@class='gotcha']")]
		[TestCase("<div> <p>waffle</p> <p class='gotcha'>foo</p> <p class='gotcha'>foo</p> </div>", "waffle")]
		[TestCase("<div> <p></p> <img></img> <p class='gotcha'>waffle</p> <p class='gotcha'>foo</p> </div>", "", "*[@class='gotcha']", "img")]
		public void CollectionOfXElement(string html, string outcome, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.TakeFirst(patterns);
			var elements = (SupportedCommands.XPath().Run(GetContext(), html) as XDocument)?.XPathSelectElements("//div/*").ToList();

			//Act
			var result = (XElement)command.Run(GetContext(), elements);

			//Assert
			if (outcome == null)
			{
				result.Should().BeNull();
			}
			else
			{
				result.Should().NotBeNull();
				result.Value.Should().BeEquivalentTo(outcome);
			}
		}
	}
}