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
	internal class TakeWhileTests : ParserTestsBase
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
			var command = SupportedCommands.TakeWhile();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		private static object[] _stringCases =
		{
			new object[] {new [] {"one", "two", "three"}, 0, null},
			new object[] {new [] {"one", "two", "three"}, 0, new string[0]},
			new object[] {new [] {"one", "two", "three"}, 2, new [] {"one", "two"}},
			new object[] {new [] {"one", "two", "three"}, 0, new [] {"waffle", "two"}},
			new object[] {new [] {"one", "two", "three"}, 1, new [] {"waffle", "one"}}
		};
		[TestCaseSource(nameof(_stringCases))]
		public void CollectionOfStrings(string[] values, int resultCount, string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.TakeWhile(patterns);

			//Act
			var result = command.Run(GetContext(), values) as IEnumerable<string>;

			//Assert
			if (patterns == null || patterns.Length == 0 || resultCount == 0)
			{
				result.Should().BeNull();
			}
			else
			{
				result.Should().NotBeNull();
				result.Count().Should().Be(resultCount);
			}
		}

		[TestCase("<div> <p></p> <p></p> <p></p> <p></p> </div>", 0)]
		[TestCase("<div> <p></p> <p></p> <p></p> <p></p> </div>", 4, "p")]
		[TestCase("<div> <p class='one'></p> <p class='two'></p> <p class='three'></p> <p></p> </div>", 3, ".one", ".two", ".three")]
		[TestCase("<div> <p class='one'></p> <p class='two'></p> <p class='three'></p> <p></p> </div>", 2, ".one", ".two")]
		[TestCase("<div> <p class='one'></p> <p class='two'></p> <p class='three'></p> <p></p> </div>", 1, ".one")]
		[TestCase("<div> <p class='one'></p> <p class='two'></p> <p class='three'></p> <p></p> </div>", 3, ".three", ".one", ".two")]
		[TestCase("<div> <p class='boo'></p> <p class='two'></p> <p class='three'></p> <p></p> </div>", 0, ".three", ".one", ".two")]
		public void CollectionOfIElement(string htmlBody, int count, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.TakeWhile(patterns);
			var elements = GetDocument(htmlBody).QuerySelectorAll("p");

			//Act
			var result = command.Run(GetContext(), elements) as IEnumerable<IElement>;

			//Assert
			if (patterns == null || patterns.Length == 0 || count == 0)
			{
				result.Should().BeNull();
			}
			else
			{
				result.Should().NotBeNull();
				result.Count().Should().Be(count);
			}
		}

		[TestCase("<div> <p></p> <p></p> <p></p> <p></p> </div>", 0)]
		[TestCase("<div> <p></p> <p></p> <p></p> <p></p> </div>", 4, "p")]
		[TestCase("<div> <p class='one'></p> <p class='two'></p> <p class='three'></p> <p></p> </div>", 3, "*[@class='one']", "*[@class='two']", "*[@class='three']")]
		[TestCase("<div> <p class='one'></p> <p class='two'></p> <p class='three'></p> <p></p> </div>", 2, "*[@class='one']", "*[@class='two']")]
		[TestCase("<div> <p class='one'></p> <p class='two'></p> <p class='three'></p> <p></p> </div>", 1, "*[@class='one']")]
		[TestCase("<div> <p class='one'></p> <p class='two'></p> <p class='three'></p> <p></p> </div>", 3, "*[@class='three']", "*[@class='one']", "*[@class='two']")]
		[TestCase("<div> <p class='boo'></p> <p class='two'></p> <p class='three'></p> <p></p> </div>", 0, "*[@class='three']", "*[@class='one']", "*[@class='two']")]
		public void CollectionOfXElement(string html, int count, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.TakeWhile(patterns);
			var elements = (SupportedCommands.XPath().Run(GetContext(), html) as XDocument)?.XPathSelectElements("//div/*").ToList();

			//Act
			var result = command.Run(GetContext(), elements) as IEnumerable<XElement>;

			//Assert
			if (patterns == null || patterns.Length == 0 || count == 0)
			{
				result.Should().BeNull();
			}
			else
			{
				result.Should().NotBeNull();
				result.Count().Should().Be(count);
			}
		}
	}
}