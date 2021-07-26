using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Commands
{
	[Category("Commands")]
	[TestFixture]
	internal class DistinctTests : ParserTestsBase
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
			var command = SupportedCommands.Distinct();

			//Act
			var result = command.Run(GetContext(), value);

			//Assert
			result.Should().BeNull();
		}

		
		public void ShouldPassThrough(object data)
		{
			//Arrange
			var command = SupportedCommands.Distinct();

			//Act
			var result = command.Run(GetContext(), data);

			//Assert
			result.Should().NotBeNull();
			result.Should().Be(data);
		}

		[TestCase(1, "one", "one", "one")]
		[TestCase(2, "one", "two", "one")]
		[TestCase(3, "one", "two", "three")]
		public void CollectionOfStrings(int outcomeCount, params string[] values)
		{
			//Assert
			var command = SupportedCommands.Distinct();

			//Act
			var result = (command.Run(GetContext(), values) as IEnumerable<string> ?? Enumerable.Empty<string>()).ToArray();

			//Assert
			result.Length.Should().Be(outcomeCount);
		}
	}
}