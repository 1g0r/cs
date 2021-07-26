using System;
using AngleSharp.Dom;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	internal class CssTests : ParserTestsBase
	{
		private static object[] _cases =
		{
			666,
			DateTimeOffset.Now,
			"This is not HTML",
			"",
			null,
			new[] { 1, 2, 3 },
			typeof(AttrTests)
		};
		[TestCaseSource(nameof(_cases))]
		public void NotSupportedValueType(object value)
		{
			//Arrange
			var command = SupportedCommands.Css();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		
		[TestCase("")]
		[TestCase("<div> <p></p> <p></p> <p></p> </div>")]
		public void TakesString(string htmlBody)
		{
			//Arrange
			var command = SupportedCommands.Css();
			var html = FormatHtml(htmlBody);

			//Act
			var result = (IDocument)command.Run(GetContext(), html);

			//Assert
			result.Should().NotBeNull();
			result.Title.Should().BeEquivalentTo("The Title");
		}


	}
}