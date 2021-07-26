using System.Collections;
using System.Collections.Generic;
using Mindscan.Media.HtmlParser.Core.Helpers;

namespace Mindscan.Media.HtmlParser.Core.Expression
{
	[CustomJsonObject(SupportedNames.Expressions.Namespace, SupportedNames.Expressions.And)]
	internal sealed class AndExpression : ExpressionBase, IExpression
	{
		public object Evaluate(ExpressionContext context, object data)
		{
			var result = new List<object>();
			foreach (var expression in Expressions)
			{
				dynamic value = expression.Evaluate(context, data);
				if (value != null)
				{
					if (!(value is string) && (value is ICollection || value is IEnumerable))
					{
						result.AddRange(value);
					}
					else
					{
						result.Add(value);
					}
				}
			}
			return result.ToResult();
		}
	}
}
