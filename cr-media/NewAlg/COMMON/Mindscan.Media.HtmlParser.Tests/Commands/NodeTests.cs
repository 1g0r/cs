using System;
using System.Xml.Linq;
using AngleSharp.Dom;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	internal class NodeTests : ParserTestsBase
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
			var command = SupportedCommands.Node();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("<div><p /><p/><a/></div>", null)]
		[TestCase("<div><p /><p/><a/></div>", null, "img", ".gotcha")]
		[TestCase("<div><p class='gotcha'>waffle</p><p/><a class='gotcha'/></div>", "waffle", "p.gotcha", ".gotcha")]
		[TestCase("<div><p class='gotcha'>waffle</p><p/><a class='gotcha'/></div>", "", "img", "a.gotcha")]
		[TestCase("<div><p class='gotcha'>waffle</p><p/><a class='gotcha'/></div>", "waffle", ".gotcha")]
		public void TakesIParentNode(string htmlBody, string value, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.Node(patterns);
			var doc = GetDocument(htmlBody);

			//Act
			var node = (IElement)command.Run(GetContext(), doc);
			var result = node?.TextContent;

			//Assert
			result.Should().BeEquivalentTo(value);
		}

		[TestCase("<div><p /><p/><a/></div>", null)]
		[TestCase("<div><p /><p/><a/></div>", null, "//img", "//*[@class=gotcha]")]
		[TestCase("<div><p class='gotcha'>waffle</p><p/><a class='gotcha'></a></div>", "waffle", "//p[@class='gotcha']", "//*[@class='gotcha']")]
		[TestCase("<div><p class='gotcha'>waffle</p><p/><a class='gotcha'></a></div>", "", "//img", "//a[@class='gotcha']")]
		[TestCase("<div><p class='gotcha'>waffle</p><p/><a class='gotcha'></a></div>", "waffle", "//*[@class='gotcha']")]
		public void TakesXNode(string html, string value, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.Node(patterns);
			var doc = (XDocument)SupportedCommands.XPath().Run(GetContext(), html);

			//Act
			var node = (XElement)command.Run(GetContext(), doc);
			var result = node?.Value;

			//Assert
			result.Should().BeEquivalentTo(value);
		}

		[TestCase("{cards:[{type:'video', value:'waffle'}, {type:'image'}, {type:'video', value:'foo'}]}", null)]
		[TestCase("{cards:[{type:'video', value:'waffle'}, {type:'image'}, {type:'video', value:'foo'}]}", null, "$..cards.[?(@.type=='text')][0].value")]
		[TestCase("{cards:[{type:'video', value:'waffle'}, {type:'image'}, {type:'video', value:'foo'}]}", "waffle", "$..cards.[0].value")]
		[TestCase("{cards:[{type:'video', value:'foo'}, {type:'image'}, {type:'text', value:'waffle'}]}", "foo", "$..cards.[10].value", "$..cards.[0].value")]

		public void TakesJToken(string json, string outcome, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.Node(patterns);
			var jToken = JToken.Parse(json);

			//Act
			var node = command.Run(GetContext(), jToken) as JToken;
			var result = node?.Value<string>();

			//Assert
			result.Should().BeEquivalentTo(outcome);
		}
	}
}