using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	internal class JoinTests : ParserTestsBase
	{
		private static object[] _cases =
		{
			"",
			"This is not HTML",
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
			var command = SupportedCommands.Join();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		
		public void ShouldPassThrough(object data)
		{
			//Arrange
			var command = SupportedCommands.Join();

			//Act
			var result = command.Run(GetContext(), data);

			//Assert
			result.Should().NotBeNull();
			result.Should().Be(data);
		}

		[TestCase(false, "one", "two", "three", "four")]
		[TestCase(false, "one")]
		[TestCase(true, "", "", "")]
		public void TakeEnumerableOfStrings(bool isNull, params string[] values)
		{
			//Arrange
			var command = SupportedCommands.Join();

			//Act
			var result = (string)command.Run(GetContext(), values);
			var testValue = string.Join(Environment.NewLine, values.Where(x => !string.IsNullOrEmpty(x)));

			//Assert
			if (isNull)
			{
				result.Should().BeNull();
			}
			else
			{
				result.Should().BeEquivalentTo(testValue);
			}
		}

		[TestCase(" ")]
		[TestCase(null)]
		[TestCase("")]
		[TestCase("XXXX")]
		public void CustomDelimiters(string delimiter)
		{
			//Arrange 
			var values = new List<string> { "one", "two", "three" };
			delimiter = string.IsNullOrEmpty(delimiter) ? Environment.NewLine : delimiter;
			var command = SupportedCommands.Join(delimiter);


			//Act
			var result = (string)command.Run(GetContext(), values);
			var testValue = string.Join(delimiter, values);

			//Assert
			result.Should().BeEquivalentTo(testValue);
		}
	}
}