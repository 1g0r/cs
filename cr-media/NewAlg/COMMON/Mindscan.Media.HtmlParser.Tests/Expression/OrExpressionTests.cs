using FluentAssertions;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Tests.Stubs;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Expression
{
	[Category("UnitTests")]
	public class OrExpressionTests : ParserTestsBase
	{
		[Test]
		public void TakesPipelines()
		{
			//Arrange
			var context = new ExpressionContext(GetContext());
			var nullCommand = new NullCommandStub();
			var passThroughCommand = new PassThroughCommandStub();
			var expression = ExpressionBuilder.Or(
				ExpressionBuilder.Pipeline(nullCommand),
				ExpressionBuilder.Pipeline(nullCommand),
				ExpressionBuilder.Pipeline(passThroughCommand),
				ExpressionBuilder.Pipeline(nullCommand)
			);

			//Act
			var result = expression.Evaluate(context, "foo");

			//Assert
			result.Should().NotBeNull();
			result.Should().BeEquivalentTo("foo");
			nullCommand.ExecuteCount.Should().Be(2);
			passThroughCommand.ExecuteCount.Should().Be(1);
		}

		[Test]
		public void TakesExpressions()
		{
			//Arrange
			var context = new ExpressionContext(GetContext());
			var nullCommand = new NullCommandStub();
			var passThroughCommand = new PassThroughCommandStub();
			var expression = ExpressionBuilder.Or(
				ExpressionBuilder.Pipeline(nullCommand),
				ExpressionBuilder.Or(
					ExpressionBuilder.Pipeline(nullCommand),
					ExpressionBuilder.Pipeline(passThroughCommand)
				),
				ExpressionBuilder.Pipeline(nullCommand)
			);

			//Act
			var result = expression.Evaluate(context, "foo");

			//Assert
			result.Should().NotBeNull();
			result.Should().BeEquivalentTo("foo");
			nullCommand.ExecuteCount.Should().Be(2);
			passThroughCommand.ExecuteCount.Should().Be(1);
		}

		[Test]
		public void ShouldNotDisposeContext()
		{
			//Arrange
			var outerValue = new DisposableValueStub();
			var disposableCommand = new DisposableCommandStub();
			var pipe = ExpressionBuilder.Or(
				ExpressionBuilder.Pipeline(disposableCommand),
				ExpressionBuilder.Pipeline(disposableCommand),
				ExpressionBuilder.Pipeline(disposableCommand));
			var context = new ExpressionContext(GetContext());

			//Act
			var result = pipe.Evaluate(context, outerValue);

			//Assert
			result.Should().NotBeNull();
			outerValue.DisposeCount.Should().Be(0);
			disposableCommand.Disposables.Count.Should().Be(1);
			disposableCommand.Disposables[0].DisposeCount.Should().Be(0);
		}
	}
}