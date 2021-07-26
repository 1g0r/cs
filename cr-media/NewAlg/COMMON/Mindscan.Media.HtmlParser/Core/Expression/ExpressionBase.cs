using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Expression
{
	internal abstract class ExpressionBase: CustomJsonObject
	{
		public const string EXPRESSIONS_NAME = "$expressions";

		[JsonProperty(EXPRESSIONS_NAME, Order = 1)]
		public List<IExpression> Expressions { get; } = new List<IExpression>();
	}
}