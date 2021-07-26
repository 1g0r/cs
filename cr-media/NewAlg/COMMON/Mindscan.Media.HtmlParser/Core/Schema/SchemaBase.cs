using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mindscan.Media.HtmlParser.Core.Schema
{
	internal abstract class SchemaBase : CustomJsonObject, ISchema
	{
		public const string EXPRESSION_NAME = "$expression";

		[JsonProperty(EXPRESSION_NAME, Order = 1)]
		public IExpression Expression { get; set; }
		public JToken Parse(ParserContext context, object data)
		{
			using (var expressionContext = new ExpressionContext(context))
			{
				var result = Expression == null ? data : Expression.Evaluate(expressionContext, data);
				if (result != null)
				{
					return EvaluateSchema(expressionContext, result);
				}
				return null;
			}
		}

		protected abstract JToken EvaluateSchema(ExpressionContext context, object data);
	}
}