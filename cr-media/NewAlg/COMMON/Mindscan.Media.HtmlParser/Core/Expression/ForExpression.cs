namespace Mindscan.Media.HtmlParser.Core.Expression
{
	[CustomJsonObject(SupportedNames.Expressions.Namespace, SupportedNames.Expressions.For)]
	internal sealed class ForExpression : ExpressionBase, IExpression
	{
		public object Evaluate(ExpressionContext context, object data)
		{
			if (data == null || Expressions == null || Expressions.Count == 0)
				return null;
			foreach (var expression in Expressions)
			{
				data = expression.Evaluate(context, data);
				if (data == null)
					return null;

				//context.AddIfDisposable(data);
			}

			return data;
		}
	}
}