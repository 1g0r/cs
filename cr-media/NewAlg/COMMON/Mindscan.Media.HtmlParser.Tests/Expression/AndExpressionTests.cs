using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Tests.Stubs;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Expression
{
	[Category("UnitTests")]
	public class AndExpressionTests: ParserTestsBase
	{
		[Test]
		public void TakesAllPipelines()
		{
			//Arrange
			var context = new ExpressionContext(GetContext());
			var nullCommand = new NullCommandStub();
			var passThroughCommand = new PassThroughCommandStub();
			var listCommand = new ListCommandStub(3);
			var expression = ExpressionBuilder.And(
				ExpressionBuilder.Pipeline(nullCommand),
				ExpressionBuilder.Pipeline(passThroughCommand),
				ExpressionBuilder.Pipeline(nullCommand),
				ExpressionBuilder.Pipeline(passThroughCommand),
				ExpressionBuilder.Pipeline(nullCommand),
				ExpressionBuilder.Pipeline(listCommand)
			);

			//Act
			var result = expression.Evaluate(context, "foo");
			var arr = (result as IEnumerable<dynamic>)?.Cast<string>().ToArray() ?? new string[0];

			//Assert
			result.Should().NotBeNull();
			nullCommand.ExecuteCount.Should().Be(3);
			passThroughCommand.ExecuteCount.Should().Be(2);
			listCommand.ExecuteCount.Should().Be(1);
			arr.Should().NotBeNullOrEmpty();
			arr.Length.Should().Be(2 + 3);
			foreach (var item in arr)
			{
				item.Should().BeEquivalentTo("foo");
			}
		}

		[Test]
		public void TakesExpressions()
		{
			//Arrange
			var context = new ExpressionContext(GetContext());
			var nullCommand = new NullCommandStub();
			var passThroughCommand = new PassThroughCommandStub();
			var listCommand = new ListCommandStub(3);
			var expression = ExpressionBuilder.And(
				ExpressionBuilder.Pipeline(nullCommand),
				ExpressionBuilder.Pipeline(passThroughCommand),
				ExpressionBuilder.Pipeline(nullCommand),
				ExpressionBuilder.Pipeline(passThroughCommand),
				ExpressionBuilder.Pipeline(nullCommand),
				ExpressionBuilder.And(
					ExpressionBuilder.Pipeline(nullCommand),
					ExpressionBuilder.Pipeline(passThroughCommand),
					ExpressionBuilder.Pipeline(nullCommand),
					ExpressionBuilder.Pipeline(passThroughCommand),
					ExpressionBuilder.Pipeline(nullCommand),
					ExpressionBuilder.Pipeline(listCommand)
				)
			);

			//Act
			var result = expression.Evaluate(context, "foo");
			var arr = (result as IEnumerable<dynamic>)?.Cast<string>().ToArray() ?? new string[0];

			//Assert
			result.Should().NotBeNull();
			nullCommand.ExecuteCount.Should().Be(6);
			passThroughCommand.ExecuteCount.Should().Be(4);
			listCommand.ExecuteCount.Should().Be(1);
			arr.Should().NotBeNullOrEmpty();
			arr.Length.Should().Be(7);
			foreach (var item in arr)
			{
				item.Should().BeEquivalentTo("foo");
			}
		}

		[Test]
		public void ShouldNotDisposeContext()
		{
			//Arrange
			var outerValue = new DisposableValueStub();
			var disposableCommand = new DisposableCommandStub();
			var pipe = ExpressionBuilder.And(
				ExpressionBuilder.Pipeline(disposableCommand),
				ExpressionBuilder.Pipeline(disposableCommand),
				ExpressionBuilder.Pipeline(disposableCommand));
			var context = new ExpressionContext(GetContext());

			//Act
			var result = pipe.Evaluate(context, outerValue);

			//Assert
			result.Should().NotBeNull();
			outerValue.DisposeCount.Should().Be(0);
			disposableCommand.Disposables.Count.Should().Be(3);
			disposableCommand.Disposables[0].DisposeCount.Should().Be(0);
			disposableCommand.Disposables[1].DisposeCount.Should().Be(0);
			disposableCommand.Disposables[2].DisposeCount.Should().Be(0);
		}
	}
}