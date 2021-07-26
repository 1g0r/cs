using FluentAssertions;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Schema;
using Mindscan.Media.HtmlParser.Tests.Stubs;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Schema
{
	public class SimpleValueTests : ParserTestsBase
	{
		[Test]
		public void ShouldDisposeOnlyInternal()
		{
			//Arrange
			var outerValue = new DisposableValueStub();
			var disposableCommand = new DisposableCommandStub();
			var passThrough = new PassThroughCommandStub();
			
			var schema = new SimpleValue
			{
				Expression = ExpressionBuilder.Pipeline(
					disposableCommand,
					disposableCommand,
					disposableCommand,
					passThrough,
					passThrough
				)
			};

			//Act
			var result = schema.Parse(GetContext(), outerValue);

			//Assert
			result.Should().BeNull();
			passThrough.ExecuteCount.Should().Be(2);
			// outer value shouldn't be disposed
			outerValue.DisposeCount.Should().Be(0);
			disposableCommand.Disposables.Count.Should().Be(3);
			foreach (var disposable in disposableCommand.Disposables)
			{
				disposable.DisposeCount.Should().Be(1);
				disposable.Should().NotBe(outerValue);
			}
		}

		[TestCase(new object[] {1, 2, 3}, true)]
		[TestCase(new object[] {" ", " ", " "}, true)]
		[TestCase("waffle", false)]
		[TestCase(666, false)]
		public void ShouldReturnNull(object value, bool isNull)
		{
			//Arrange
			var passThrough = new PassThroughCommandStub();
			var schema = new SimpleValue
			{
				Expression = ExpressionBuilder.Pipeline(passThrough)
			};

			//Act
			var result = schema.Parse(GetContext(), value) == null;

			//Assert
			result.Should().Be(isNull);
		}
	}
}
