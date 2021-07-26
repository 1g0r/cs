using FluentAssertions;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Schema;
using Mindscan.Media.HtmlParser.Tests.Stubs;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Schema
{
	public class ArrayValueTests : ParserTestsBase
	{
		[Test]
		public void ShouldDisposeOnlyInternal()
		{
			//Arrange
			var outerValue = new DisposableValueStub();
			var disposableCommand = new DisposableCommandStub();
			var passThrough = new PassThroughCommandStub();

			var schema = new ArrayValue
			{
				Expression = ExpressionBuilder.Pipeline(
					disposableCommand,
					disposableCommand,
					disposableCommand,
					passThrough,
					passThrough
				),
				Item = new SimpleValue
				{
					Expression = ExpressionBuilder.Pipeline(disposableCommand, passThrough)
				}
			};

			//Act
			var result = schema.Parse(GetContext(), outerValue);

			//Assert
			result.Should().BeNull();
			passThrough.ExecuteCount.Should().Be(3);
			// outer value shouldn't be disposed
			outerValue.DisposeCount.Should().Be(0);
			disposableCommand.Disposables.Count.Should().Be(4);
			foreach (var disposable in disposableCommand.Disposables)
			{
				disposable.DisposeCount.Should().Be(1);
				disposable.Should().NotBe(outerValue);
			}
		}

		[TestCase(new object[] { 1, 2, 3 }, false)]
		[TestCase(new object[] { " ", " ", " " }, false)]
		[TestCase("waffle", false)]
		[TestCase(666, false)]
		[TestCase(new object[] {null, null}, true)]
		[TestCase(new object[] {typeof(PassThroughCommandStub) }, true)]
		public void ShouldReturnNull(object value, bool isNull)
		{
			//Arrange
			var passThrough = new PassThroughCommandStub();
			var schema = new ArrayValue
			{
				Expression = ExpressionBuilder.Pipeline(passThrough),
				Item = new SimpleValue
				{
					Expression = ExpressionBuilder.Pipeline(passThrough)
				}
			};

			//Act
			var jToken = schema.Parse(GetContext(), value);
			var result = jToken == null;

			//Assert
			result.Should().Be(isNull);
		}
	}
}