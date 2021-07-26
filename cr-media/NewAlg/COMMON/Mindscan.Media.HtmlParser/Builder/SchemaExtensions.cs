using System.Collections.Generic;
using Mindscan.Media.HtmlParser.Core.Expression;
using Mindscan.Media.HtmlParser.Core.Parser;
using Mindscan.Media.HtmlParser.Core.Schema;

namespace Mindscan.Media.HtmlParser.Builder
{
	public static class SchemaExtensions
	{
		public static IPageParser Encoding(this IPageParser parser, string encoding)
		{
			var tmp = parser as HtmlPageParser;
			if (tmp != null)
			{
				tmp.Encoding = encoding;
			}
			return parser;
		}

		public static IPageParser Schema(this IPageParser parser, IExpression expression, params KeyValuePair<string, ISchema>[] properties)
		{
			var pb = parser as HtmlPageParser;
			if (pb != null && properties != null)
			{
				var value = new ComplexValue();
				foreach (var property in properties)
				{
					value.Properties.Add(property.Key, property.Value);
				}
				pb.Schema = value;
				if (expression != null)
				{
					pb.Schema.Expression = expression;
				}
			}
			return parser;
		}

		public static KeyValuePair<string, ISchema> SimpleValue(this string name, IExpression expression)
		{
			var value = new SimpleValue();
			if (expression != null)
			{
				value.Expression = expression;
			}
			return new KeyValuePair<string, ISchema>(name, value);
		}

		public static KeyValuePair<string, ISchema> SimpleValue(this string name, params IPipelineCommand[] commands)
		{
			var value = new SimpleValue();
			if (commands != null)
			{
				var pipeline = new PipelineExpression();
				foreach (var command in commands)
				{
					pipeline.Commands.Add(command);
				}
				value.Expression = pipeline;
			}
			return new KeyValuePair<string, ISchema>(name, value);
		}

		public static KeyValuePair<string, ISchema> ComplexValue(this string name, params KeyValuePair<string, ISchema>[] properties)
		{
			var value = new ComplexValue();
			if (properties != null)
			{
				foreach (var property in properties)
				{
					value.Properties.Add(property.Key, property.Value);
				}
			}
			return new KeyValuePair<string, ISchema>(name, value);
		}

		public static KeyValuePair<string, ISchema> ArrayValue(this string name, IExpression expression, params KeyValuePair<string, ISchema>[] properties)
		{
			var result = new ArrayValue();
			if (properties != null)
			{
				var item = new ComplexValue();
				foreach (var property in properties)
				{
					item.Properties.Add(property.Key, property.Value);
				}
				result.Item = item;
			}
			else
			{
				result.Item = new SimpleValue();
			}
			if (expression != null)
			{
				result.Expression = expression;
			}
			return new KeyValuePair<string, ISchema>(name, result);
		}

		public static KeyValuePair<string, ISchema> ArrayValue(this string name, IExpression expression, params IPipelineCommand[] commands)
		{
			var result = new ArrayValue
			{
				Item = new SimpleValue(),
				Expression = expression
			};
			if (commands != null)
			{
				var pipe = new PipelineExpression();
				pipe.Commands.AddRange(commands);
				result.Item = new SimpleValue
				{
					Expression = pipe
				};
			}
			return new KeyValuePair<string, ISchema>(name, result);
		}
	}
}
