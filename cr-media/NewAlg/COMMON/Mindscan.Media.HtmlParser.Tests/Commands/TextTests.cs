using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	internal class TextTests : ParserTestsBase
	{
		private static object[] _cases =
		{
			"",
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
			var command = SupportedCommands.Text();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("")]
		[TestCase("    ")]
		[TestCase("\t\r\n\t\r\n\t")]
		public void EmptyString(string value)
		{
			//Arrange
			var command = SupportedCommands.Text();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("<div><p alt='Test'></p></div>")]
		public void EmptyIElement(string htmlBody)
		{
			//Arrange
			var command = SupportedCommands.Text();
			var element = GetDocument(htmlBody).QuerySelector("p");

			//Act
			var result = (string)command.Run(GetContext(), element);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("<div><p alt='Test'></p></div>")]
		public void EmptyXElement(string htmlBody)
		{
			//Arrange
			var command = SupportedCommands.Text();
			var element = (SupportedCommands.XPath().Run(GetContext(), htmlBody) as XDocument)?.XPathSelectElement("//p");

			//Act
			var result = (string)command.Run(GetContext(), element);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("\tff\t  \t \t\t\t\n\r fff\r\n", "ff fff")]
		[TestCase("\tff\t  \u00A0 \u200B fff\r\n", "ff fff")]
		[TestCase("   fffff", "fffff")]
		[TestCase("   ff   fff    ", "ff fff")]
		public void DirtyString(string dirtyString, string cleanString)
		{
			//Arrange
			var command = SupportedCommands.Text();

			//Act
			var result = (string)command.Run(GetContext(), dirtyString);

			//Assert
			result.Should().BeEquivalentTo(cleanString);
		}

		[TestCase(
			"<div> <p>   Waffle  \n\r is \r\t<iframe>Iframe content</iframe> foo <span>.</span>  </p> <p> <br/>  <script>script content</script>But something \"can\"\r\n go wrong</p> <style>Style content</style></div>",
			new[] { "Waffle is foo.", "But something \"can\" go wrong" })]
		[TestCase(
			"<div><p>\"<span>   Waffle\"</span> \n\r is \r\tfoo <span>.</span>  </p></div>",
			new[] { "\"Waffle\" is foo." })]
		[TestCase(
			"<p>part 1 <strong>\"part 2\"</strong> part 3 <strong>«part 4»</strong> part 5</p>",
			new[] { "part 1 \"part 2\" part 3 «part 4» part 5" })]
		[TestCase(
			"<p>part 1 \"<strong>part 2</strong>\" part 3 «<strong>part 4</strong>» part 5</p>",
			new[] { "part 1 \"part 2\" part 3 «part 4» part 5" })]
		[TestCase("<p>\"<strong>part 1<strong>\" part 2", new[] { "\"part 1\" part 2" })]
		[TestCase(
			"<p><strong>\"part 1\"<strong> <strong>\"part 2\"</strong>",
			new[] { "\"part 1\" \"part 2\"" })]
		[TestCase(
			"<p>\"part 1\" <strong>\"part 2\"</strong> «part 3» <b> </b> «<strong>part 4</strong>» \"part 5\"</p>",
			new[] { "\"part 1\" \"part 2\" «part 3» «part 4» \"part 5\"" })]
		[TestCase(
			"<p>part 1 <strong>boo</strong></p><p>Part 2</p>",
			new[] { "part 1", "Part 2" },
			"div", "strong")]
		public void CollectionOfIElement(string htmlBody, string[] outcome, params string[] skipPatterns)
		{
			//Arrange
			var command = SupportedCommands.Text(skipPatterns);
			var elements = GetDocument(htmlBody).QuerySelectorAll("p").ToList();

			//Act
			var result = (List<string>)command.Run(GetContext(), elements);

			//Assert
			result.Should().BeEquivalentTo(outcome);

		}

		[TestCase("<div> <p>   Waffle  \n\r is \r\t<iframe>Iframe content</iframe> foo <span>.</span>  </p> <p> <br/>  <script>script content</script>But something \"can\"\r\n go wrong</p> <style>Style content</style></div>", "Waffle is foo.")]
		[TestCase("<div><p>\"<span>   Waffle\"</span> \n\r is \r\tfoo <span>.</span>  </p></div>", "\"Waffle\" is foo.")]
		[TestCase("<p>part 1 <strong>\"part 2\"</strong> part 3 <strong>«part 4»</strong> part 5</p>", "part 1 \"part 2\" part 3 «part 4» part 5")]
		[TestCase("<p>part 1 \"<strong>part 2</strong>\" part 3 «<strong>part 4</strong>» part 5</p>", "part 1 \"part 2\" part 3 «part 4» part 5")]
		[TestCase("<p>\"<strong>part 1<strong>\" part 2", "\"part 1\" part 2")]
		[TestCase("<p><strong>\"part 1\"<strong> <strong>\"part 2\"</strong>", "\"part 1\" \"part 2\"")]
		[TestCase("<p>\"part 1\" <strong>\"part 2\"</strong> «part 3» <b> </b> «<strong>part 4</strong>» \"part 5\"</p>", "\"part 1\" \"part 2\" «part 3» «part 4» \"part 5\"")]
		[TestCase("<p>part 1 <strong>boo</strong></p>", "part 1", "div", "strong")]
		[TestCase("<p>&nbsp;line&nbsp;1 line    2 line 3 line   4 &nbsp; &nbsp;   </p>", "line 1 line 2 line 3 line 4")]
		public void TakesIElement(string htmlBody, string outcome, params string[] skipPatterns)
		{
			//Arrange
			var command = SupportedCommands.Text(skipPatterns);
			var elements = GetDocument(htmlBody).QuerySelector("p");

			//Act
			var result = (string)command.Run(GetContext(), elements);

			//Assert
			result.Should().BeEquivalentTo(outcome);
		}

		[TestCase("<div><p>part 1 <strong>boo</strong></p></div>", "part 1", "div", "strong")]
		[TestCase("<div> <p>   Waffle  \n\r is \r\t<iframe>Iframe content</iframe> foo <span>.</span>  </p> <p> <br/>  <script>script content</script>But something \"can\"\r\n go wrong</p> <style>Style content</style></div>", "Waffle is foo.")]
		[TestCase("<div><p>\"<span>   Waffle\"</span> \n\r is \r\tfoo <span>.</span>  </p></div>", "\"Waffle\" is foo.")]
		[TestCase("<div><p>part 1 <strong>\"part 2\"</strong> part 3 <strong>«part 4»</strong> part 5</p></div>", "part 1 \"part 2\" part 3 «part 4» part 5")]
		[TestCase("<div><p>part 1 \"<strong>part 2</strong>\" part 3 «<strong>part 4</strong>» part 5</p></div>", "part 1 \"part 2\" part 3 «part 4» part 5")]
		[TestCase("<root><p>\"<strong>part 1</strong>\" part 2</p></root>", "\"part 1\" part 2")]
		[TestCase("<div><p><strong>\"part 1\"</strong> <strong>\"part 2\"</strong></p></div>", "\"part 1\" \"part 2\"")]
		[TestCase("<div><p>\"part 1\" <strong>\"part 2\"</strong> «part 3» <b> </b> «<strong>part 4</strong>» \"part 5\"</p></div>", "\"part 1\" \"part 2\" «part 3» «part 4» \"part 5\"")]
		[TestCase("<div><p>&nbsp;line&nbsp;1 line    2 line 3 line   4 &nbsp; &nbsp;   </p></div>", "line 1 line 2 line 3 line 4")]
		public void TakesXElement(string html, string outcome, params string[] skipPatterns)
		{
			//Arrange
			var command = SupportedCommands.Text(skipPatterns);
			var elements = (SupportedCommands.XPath().Run(GetContext(), html) as XDocument)?.XPathSelectElement("//p");

			//Act
			var result = (string)command.Run(GetContext(), elements);

			//Assert
			result.Should().BeEquivalentTo(outcome);
		}

		[TestCase("{ data:[{data:'waffle'}]}", "waffle")]
		[TestCase("{ data:[{type:'waffle'}]}", null)]
		public void TakesJToken(string json, string outcome)
		{
			//Arrange
			var command = SupportedCommands.Text();
			var jToken = JToken.Parse(json).SelectToken("$..data[0].data");

			//Act
			var result = (string)command.Run(GetContext(), jToken);

			//Assert
			result.Should().BeEquivalentTo(outcome);
		}
	}
}