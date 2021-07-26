using System;
using System.Xml.Linq;
using AngleSharp.Dom;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	class AttrTests : ParserTestsBase
	{
		[TestCase(null)]
		[TestCase(typeof(AttrTests))]
		[TestCase("")]
		[TestCase(666)]
		public void NotSupportedContextType(object value)
		{
			//Arrange
			var command = SupportedCommands.Attr();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("<div href='/foo/poo/waffle.html'></div>", null, "href")]
		[TestCase("<a href='/foo/poo/waffle.html'></a>", null, "src", "customAttr")]
		[TestCase("<a href='/foo/poo/waffle.html'></a>", null)]
		[TestCase("<a src='' href='/foo/poo/waffle.html'></a>", BaseUrl + "/foo/poo/waffle.html", "src", "href")]
		[TestCase("<a src='  ' href='/foo/poo/waffle.html'></a>", BaseUrl + "/foo/poo/waffle.html", "src", "href")]
		[TestCase("<a src='  /foo/poo/waffle.html'></a>", BaseUrl + "/foo/poo/waffle.html", "src", "href")]
		[TestCase("<a customAttr='/foo/poo/waffle.html'></a>", "/foo/poo/waffle.html", "src", "customAttr")]
		[TestCase("<a customAttr='/foo/poo/waffle.html'></a>", "/foo/poo/waffle.html")]
		public void MatchIElement(string elementHtml, string value, params string[] names)
		{
			//Arrange
			var command = SupportedCommands.Attr(names);
			var element = GetElement(elementHtml, "body a");

			//Act
			var result = command.Run(GetContext(), element) as string;

			//Arrange
			if (names.Length == 0)
			{
				result.Should().BeNull();
			}
			else
			{
				result.Should().BeEquivalentTo(value);
			}
		}

		[TestCase("<div>ShiKaka</div>", null, "waffle")]
		[TestCase("<div src='/foo/poo/waffle.html'>ShiKaka</div>", null)]
		[TestCase("<div attr='someValue'>ShiKaka</div>", null, "src", "customAttr")]
		[TestCase("<a src='' href='/foo/poo/waffle.html'></a>", "/foo/poo/waffle.html", "src", "href")]
		[TestCase("<a src='  ' href='/foo/poo/waffle.html'></a>", "/foo/poo/waffle.html", "src", "href")]
		[TestCase("<a src='  /foo/poo/waffle.html'></a>", "/foo/poo/waffle.html", "src", "href")]
		[TestCase("<div customAttr='/foo/poo/waffle.html'></div>", "/foo/poo/waffle.html", "src", "customAttr")]
		[TestCase("<div customAttr='/foo/poo/waffle.html'></div>", "/foo/poo/waffle.html")]
		public void MatchXElement(string html, string value, params string[] names)
		{
			//Arrange
			var command = SupportedCommands.Attr(names);
			var doc = SupportedCommands.XPath().Run(GetContext(), html) as XDocument;
			var element = doc.Root;

			//Act
			var result = command.Run(GetContext(), element) as string;

			//Assert
			if (names.Length == 0)
			{
				result.Should().BeNull();
			}
			else
			{
				result.Should().BeEquivalentTo(value);
			}
		}

		private static IElement GetElement(string htmlBody, string selector)
		{
			var doc = GetDocument(htmlBody);
			return doc.QuerySelector(selector);
		}
	}
}