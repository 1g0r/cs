namespace Mindscan.Media.HtmlParser.Core.Expression
{
	[CustomJsonObject(SupportedNames.Expressions.Namespace, SupportedNames.Expressions.Or)]
	internal sealed class OrExpression : ExpressionBase, IExpression
	{
		public object Evaluate(ExpressionContext context, object data)
		{
			foreach (var expression in Expressions)
			{
				var result = expression.Evaluate(context, data);
				if (result != null)
				{
					return result;
				}
			}
			return null;
		}
	}
}