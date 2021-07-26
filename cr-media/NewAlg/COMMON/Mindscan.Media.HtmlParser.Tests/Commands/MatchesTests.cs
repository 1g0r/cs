using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	class MatchesTests : ParserTestsBase
	{
		private static object[] _cases =
		{
			666,
			DateTimeOffset.Now,
			null,
			typeof(AttrTests)
		};
		[TestCaseSource(nameof(_cases))]
		public void NotSupportedContextType(object value)
		{
			//Arrange
			var command = SupportedCommands.Matches(new[] { "foo" });

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("data-src: \"waffle\", data-src: \"foo\", description: bar", "data-src:\\s*\"(?<url>.*?)\"", "url", new[] { "waffle", "foo" })]
		[TestCase("data-src: \"waffle\", data-src: \"foo\", description: bar", "data-src:\\s*\"(?<url>.*?)\"", "FOO", null)]
		[TestCase("data-src: \"waffle\", data-src: \"foo\", description: bar", "data-src:\\s*\"(?<url>.*?)\"", "", new[]{ "data-src: \"waffle\"", "data-src: \"foo\"" })]
		[TestCase("data-src: \"waffle\", data-src: \"foo\", description: bar", "data-src:\\s*\"(?<url>.*?)\"", null, new[]{ "data-src: \"waffle\"", "data-src: \"foo\"" })]
		public void TakesPatternAndGroup(string value, string pattern, string groupName, string[] outcome)
		{
			//Arrange
			var command = SupportedCommands.Matches(new[] { "^shikaka", pattern }, groupName);

			//Act
			var result = (List<string>)command.Run(GetContext(), value);

			//Assert
			result.Should().BeEquivalentTo(outcome);
		}
	}
}