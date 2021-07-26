using System;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	class ToFakeUrlTests : ParserTestsBase
	{
		private static object[] _cases =
		{
			"",
			666,
			DateTimeOffset.Now,
			null,
			typeof(AttrTests)
		};

		[TestCaseSource(nameof(_cases))]
		public void NotSupportedContextType(object value)
		{
			//Arrange
			var command = SupportedCommands.ToFakeUrl();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("http://site.ru", "http://site.ru/#-1114175904", "shikaka")]
		[TestCase("http://site.ru", "http://site.ru/#-1630631334", "abirvalg123")]
		[TestCase("http://site.ru", "http://site.ru/#-556144138", "go to samara plz")]
		public void TakesPattern(string url, string outcome, string param)
		{
			//Arrange
			var command = SupportedCommands.ToFakeUrl();

			//Act
			var result = (Uri)command.Run(GetContext(url), param);

			//Assert
			result.Should().BeEquivalentTo(new Uri(outcome));
		}
	}
}