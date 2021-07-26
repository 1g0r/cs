using System;
using FluentAssertions;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Commands;
using Mindscan.Media.HtmlParser.Tests.Stubs;
using NUnit.Framework;

namespace Mindscan.Media.HtmlParser.Tests.Expression
{
	[Category("UnitTests")]
	public class PipelineExpressionTests: ParserTestsBase
	{
		[Test]
		public void ExpressionsShouldNotCreateNewContext()
		{
			//Arrange
			var pageUrl = new Uri("http://test.com");
			var expressions = GetImplementations<IExpression>();
			var context = new ExpressionContext(new ParserContext(pageUrl, true));
			var data = "foo";

			foreach (var expression in expressions)
			{
				//Act
				var result = expression.Evaluate(context, data);

				//Assert
				result.Should().BeNull();
			}
		}

		[Test]
		public void ShouldNotDisposeContext()
		{
			//Arrange
			var outerValue = new DisposableValueStub();
			var disposableCommand = new DisposableCommandStub();
			var pipe = ExpressionBuilder.Pipeline(
				disposableCommand,
				disposableCommand,
				disposableCommand);
			var context = new ExpressionContext(GetContext());

			//Act
			var result = pipe.Evaluate(context, outerValue);

			//Assert
			result.Should().NotBeNull();
			outerValue.DisposeCount.Should().Be(0);
			outerValue.DisposeCount.Should().Be(0);
			disposableCommand.Disposables.Count.Should().Be(3);
			disposableCommand.Disposables[0].DisposeCount.Should().Be(0);
			disposableCommand.Disposables[1].DisposeCount.Should().Be(0);
			disposableCommand.Disposables[2].DisposeCount.Should().Be(0);
		}

		[Test]
		public void CommandsShouldNotCreateNewContext()
		{
			//Arrange
			var commands = GetImplementations<IPipelineCommand>();
			var context = GetContext();

			//Act
			foreach (var command in commands)
			{
				var result = command.Run(context, "");

				//Assert
				if (command is Nop)
				{
					result.Should().Be("");
				}
				else
				{
					result.Should().BeNull();
				}
			}
		}
	}
}