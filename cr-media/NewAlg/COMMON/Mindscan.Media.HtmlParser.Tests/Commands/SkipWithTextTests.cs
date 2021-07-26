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
	internal sealed class SkipWithTextTests : ParserTestsBase
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
			var command = SupportedCommands.SkipWithText();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		private static object[] _cases1 =
		{
			new object[] {"<div></div>", null, null, 0},
			new object[]
			{
				"<div> <p class='first'>Waffle</p>  <p></p>  <p>Waffle</p>  <p class='gotcha'>Waffle</p> </div>",
				new [] {"a"}, null, 3
			},
			new object[]
			{
				"<div> <p class='first'></p>  <p><a href='woodoo.com'>Come to woodoo</a><a/></p>  <p>  |  </p>  <p class='gotcha'></p> </div>",
				new [] {"a"}, new [] {"^$", "\\s*\\|\\s*"}, 0
			},
			new object[]
			{
				"<div> <p class='first'>The first text</p>  <p><a href='woodoo.com'>Come to woodoo</a><a/></p>  <p> <a/> | </a> </p>  <p class='gotcha'></p> </div>",
				new [] {"a"}, new [] {"^$", "\\s*\\|\\s*"}, 1
			}
		};
		[TestCaseSource(nameof(_cases1))]
		public void CollectionOfIElements(string htmlBody, string[] skip, string[] textPatterns, int resultCount)
		{
			//Arrange
			var command = SupportedCommands.SkipWithText(skip, textPatterns);
			var elements = GetDocument(htmlBody).QuerySelectorAll("p, img");

			//Act
			var result = (command.Run(GetContext(), elements) as IEnumerable<IElement>)?.ToList();
			var count = result?.Count;

			//Assert
			if (resultCount == 0)
			{
				(result == null).Should().BeTrue();
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


		private static object[] _cases2 =
		{
			new object[] {"<div></div>", null, null, 0},
			new object[]
			{
				"<div> <p class='first'>Waffle</p>  <p></p>  <p>Waffle</p>  <p class='gotcha'>Waffle</p> </div>",
				new [] {"a"}, null, 3
			},
			new object[]
			{
				"<div> <p class='first'></p>  <p><a href='woodoo.com'>Come to woodoo</a><a/></p>  <p>  |  </p>  <p class='gotcha'></p> </div>",
				new [] {"a"}, new [] {"^$", "\\s*\\|\\s*"}, 0
			},
			new object[]
			{
				"<div> <p class='first'>The first text</p>  <p><a href='woodoo.com'>Come to woodoo</a><a/></p>  <p> <a/> | </a> </p>  <p class='gotcha'></p> </div>",
				new [] {"a"}, new [] {"^$", "\\s*\\|\\s*"}, 1
			}
		};
		[TestCaseSource(nameof(_cases2))]
		public void CollectionOfXElements(string htmlBody, string[] skip, string[] textPatterns, int resultCount)
		{
			//Arrange
			var command = SupportedCommands.SkipWithText(skip, textPatterns);
			var elements = (SupportedCommands.XPath().Run(GetContext(), htmlBody) as XDocument)?.XPathSelectElements("//p").ToList();

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
