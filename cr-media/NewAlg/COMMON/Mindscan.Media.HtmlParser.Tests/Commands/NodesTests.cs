using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AngleSharp.Dom;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	internal class NodesTests : ParserTestsBase
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
			var command = SupportedCommands.Nodes();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("<div><p /><p/><a/></div>", 0)]
		[TestCase("<div><p /><p/><a/></div>", 0, "img", ".gotcha")]
		[TestCase("<div><p class='gotcha'>waffle</p><p/><a class='gotcha'/></div>", 2, "p.gotcha", ".gotcha")]
		[TestCase("<div><p class='gotcha'>foo</p><p/><a class='gotcha'>waffle</a></div>", 1, "img", "a.gotcha")]
		[TestCase("<div><p class='gotcha'>waffle</p><p/><a class='gotcha'/></div>", 2, "img.gotcha", ".gotcha")]
		[TestCase("<div><p class='gotcha'>foo</p><p/><a class='gotcha'>waffle</a></div>", 1, "a.gotcha", "h1.gotcha")]
		public void TakesIParentNode(string htmlBody, int nodesCount, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.Nodes(patterns);
			var doc = GetDocument(htmlBody);

			//Act
			var nodes = (command.Run(GetContext(), doc) as IEnumerable<IElement>)?.ToList();

			//Assert
			if (nodesCount == 0)
			{
				nodes.Should().BeNull();
			}
			else
			{
				nodes.Should().NotBeNull();
				nodes.Count.Should().Be(nodesCount);
				nodes[0].TextContent.Should().BeEquivalentTo("waffle");
			}
		}

		[TestCase("<div><p></p><p></p><a></a></div>", 0)]
		[TestCase("<div><p></p><p></p><a></a></div>", 0, "img", "*[@class='gotcha']")]
		[TestCase("<div><p class='gotcha'>waffle</p><p></p><a class='gotcha'></a></div>", 2, "//p[@class='gotcha']", "//*[@class='gotcha']")]
		[TestCase("<div><p class='gotcha'>foo</p><p></p><a class='gotcha'>waffle</a></div>", 1, "//img", "//a[@class='gotcha']")]
		[TestCase("<div><p class='gotcha'>waffle</p><p></p><a class='gotcha'>foo</a></div>", 2, "//img[@class='gotcha']", "//*[@class='gotcha']")]
		[TestCase("<div><p class='gotcha'>foo</p><p/><a class='gotcha'>waffle</a></div>", 1, "//a[@class='gotcha']", "//h1[@class='gotcha']")]
		public void TakesXNode(string html, int nodesCount, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.Nodes(patterns);
			var doc = (XDocument)SupportedCommands.XPath().Run(GetContext(), html);

			//Act
			var nodes = (command.Run(GetContext(), doc) as IEnumerable<XObject>)?.ToList();

			//Assert
			if (nodesCount == 0)
			{
				nodes.Should().BeNull();
			}
			else
			{
				nodes.Should().NotBeNull();
				nodes.Count.Should().Be(nodesCount);
				(nodes[0] as XElement)?.Value.Should().BeEquivalentTo("waffle");
			}
		}

		[TestCase("{cards:[{type:'video', value:'waffle'}, {type:'image'}, {type:'video', value:'foo'}]}", 0)]
		[TestCase("{cards:[{type:'video', value:'waffle'}, {type:'image'}, {type:'video', value:'foo'}]}", 0, "$..cards.[?(@.type=='text')].value")]
		[TestCase("{cards:[{type:'video', value:'waffle'}, {type:'image'}, {type:'video', value:'foo'}]}", 2, "$..cards.[?(@.type=='video')].value")]
		[TestCase("{cards:[{type:'video', value:'foo'}, {type:'image'}, {type:'text', value:'waffle'}]}", 2, "$..cards.[?(@.type=='text')].value", "$..cards.[?(@.type=='video')].value")]

		public void TakesJToken(string json, int nodesCount, params string[] patterns)
		{
			//Arrange
			var command = SupportedCommands.Nodes(patterns);
			var jToken = JToken.Parse(json);

			//Act
			var nodes = (command.Run(GetContext(), jToken) as IEnumerable<JToken>)?.ToList();

			//Assert
			if (nodesCount == 0)
			{
				nodes.Should().BeNull();
			}
			else
			{
				nodes.Should().NotBeNull();
				nodes.Count.Should().Be(nodesCount);
				nodes[0].Value<string>().Should().BeEquivalentTo("waffle");
			}
		}
	}
}