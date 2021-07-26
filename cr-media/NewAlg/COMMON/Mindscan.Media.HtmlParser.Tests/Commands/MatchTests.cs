using System;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	class MatchTests : ParserTestsBase
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
			var command = SupportedCommands.Match(new []{"foo"});

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		[TestCase("URL: \"\", image: \"waffle\", max_size: \"foo\", quality: 75", "image:\\s*\"(?<url>.*?)\"", "url", "waffle")]
		[TestCase("URL: \"\", image: \"waffle\", max_size: \"foo\", quality: 75", "image:\\s*\"(?<url>.*?)\"", "FOO", null)]
		[TestCase("URL: \"\", image: \"waffle\", max_size: \"foo\", quality: 75", "image:\\s*\"(?<url>.*?)\"", "", "image: \"waffle\"")]
		[TestCase("URL: \"\", image: \"waffle\", max_size: \"foo\", quality: 75", "image:\\s*\"(?<url>.*?)\"", null, "image: \"waffle\"")]
		public void TakesPatternAndGroup(string value, string pattern, string groupName, string outcome)
		{
			//Arrange
			var command = SupportedCommands.Match(new []{"^shikaka", pattern}, groupName);

			//Act
			var result = (string)command.Run(GetContext(), value);

			//Assert
			result.Should().BeEquivalentTo(outcome);
		}
	}
}