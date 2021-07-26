using Mindscan.Media.HtmlParser.Core.Expression;

namespace Mindscan.Media.HtmlParser.Builder
{
	public static class ExpressionBuilder
	{
		public static IExpression Or(params IExpression[] expressions)
		{
			var result = new OrExpression();
			if (expressions != null)
			{
				foreach (var expression in expressions)
				{
					if (expression != null)
					{
						result.Expressions.Add(expression);
					}
				}
			}
			return result;
		}

		public static IExpression And(params IExpression[] expressions)
		{
			var result = new AndExpression();
			if (expressions != null)
			{
				foreach (var expression in expressions)
				{
					if (expression != null)
					{
						result.Expressions.Add(expression);
					}
				}
			}
			return result;
		}

		public static IExpression Pipeline(params IPipelineCommand[] commands)
		{
			return Pipeline(null, commands);
		}

		private static IExpression Pipeline(IExpression[] expressions, params IPipelineCommand[] commands)
		{
			var result = new PipelineExpression();
			if (expressions != null)
			{
				result.Expressions.AddRange(expressions);
			}
			if (commands != null)
			{
				foreach (var command in commands)
				{
					if (command != null)
					{
						result.Commands.Add(command);
					}
				}
			}
			return result;
		}

		public static IExpression For(params IExpression[] expressions)
		{
			var result = new ForExpression();
			if (expressions != null)
			{
				foreach (var expression in expressions)
				{
					if (expression != null)
					{
						result.Expressions.Add(expression);
					}
				}
			}
			return result;
		}
	}
}