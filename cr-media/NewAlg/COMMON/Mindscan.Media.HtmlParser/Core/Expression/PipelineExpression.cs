using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Expression
{
	[CustomJsonObject(SupportedNames.Expressions.Namespace, SupportedNames.Expressions.Pipeline)]
	internal sealed class PipelineExpression : ExpressionBase, IExpression
	{
		public const string COMMANDS_NAME = "$commands";
		public const string CODE_NAME = "$code";

		[JsonProperty(COMMANDS_NAME, Order = 1)]
		public List<IPipelineCommand> Commands { get; } = new List<IPipelineCommand>();

		[JsonProperty(CODE_NAME, Order = 2)]
		public string Code { get; set; } 
		public object Evaluate(ExpressionContext context, object data)
		{
			if (Commands.Count == 0)
				return null;

			var parserContext = new ParserContext(context.PageUrl, context.Debug);
			foreach (var command in Commands)
			{
				data = command.Run(parserContext, data);
				if (data == null)
					return null;

				context.AddIfDisposable(data);
			}
			return data;
		}
	}
}