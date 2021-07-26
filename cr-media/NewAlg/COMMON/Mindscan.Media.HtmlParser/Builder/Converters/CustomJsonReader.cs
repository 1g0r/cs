using System;
using Mindscan.Media.HtmlParser.Core;
using Mindscan.Media.HtmlParser.Core.Expression;
using Mindscan.Media.HtmlParser.Core.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mindscan.Media.HtmlParser.Builder.Converters
{
	internal class CustomJsonReader : JsonConverter
	{
		public override bool CanWrite => false;
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (IsParser(objectType))
				return ReadObject<IPageParser>(reader, serializer);

			if (IsSchema(objectType))
				return ReadSchemaJson(reader, serializer);

			if (IsExpression(objectType))
				return ReadExpressionJson(reader, serializer);

			if (IsCommand(objectType))
				return ReadObject<IPipelineCommand>(reader, serializer);


			return null;
		}

		public override bool CanConvert(Type objectType)
		{
			return IsParser(objectType) ||
			       IsSchema(objectType) ||
			       IsCommand(objectType) ||
			       IsExpression(objectType);
		}

		private static bool IsCommand(Type objectType)
		{
			return typeof(IPipelineCommand).IsAssignableFrom(objectType);
		}

		private static bool IsParser(Type objectType)
		{
			return typeof(IPageParser).IsAssignableFrom(objectType);
		}

		private static bool IsSchema(Type objectType)
		{
			return typeof(ISchema).IsAssignableFrom(objectType);
		}

		private static bool IsExpression(Type objectType)
		{
			return typeof(IExpression).IsAssignableFrom(objectType);
		}

		protected object ReadObject<T>(JsonReader reader, out JObject jObject) where T : class
		{
			jObject = JObject.Load(reader);
			var typeName = jObject[CustomJsonObject.TYPE_NAME]?.Value<string>();
			Type resultType;
			if (ParserBuilderHelper.TryGetType<T>(typeName, out resultType))
			{
				var result = Activator.CreateInstance(resultType);
				jObject.Remove(CustomJsonObject.TYPE_NAME);
				return result;
			}
			return null;
		}

		protected object ReadObject<T>(JsonReader reader, JsonSerializer serializer) where T : class
		{
			JObject jObject;
			var result = ReadObject<T>(reader, out jObject);
			if (result != null)
			{
				serializer.Populate(jObject.CreateReader(), result);
			}

			return result;
		}

		private object ReadSchemaJson(JsonReader reader, JsonSerializer serializer)
		{
			JObject jObject;
			var result = ReadObject<ISchema>(reader, out jObject);

			if (result != null)
			{
				ReadSchemaExpression(result as SchemaBase, jObject, serializer);
				ReadProperties((dynamic)result, jObject, serializer);
			}
			return result;
		}

		private static void ReadSchemaExpression(SchemaBase schema, JObject jSchema, JsonSerializer serializer)
		{
			var jExpression = jSchema[SchemaBase.EXPRESSION_NAME];
			if (jExpression != null)
			{
				var expression = serializer.Deserialize<IExpression>(jExpression.CreateReader());
				if (expression != null)
				{
					schema.Expression = expression;
				}

			}
			jSchema.Remove(SchemaBase.EXPRESSION_NAME);
		}

		private object ReadExpressionJson(JsonReader reader, JsonSerializer serializer)
		{
			JObject jObject;
			var result = ReadObject<IExpression>(reader, out jObject);
			if (result != null)
			{
				var pipe = result as PipelineExpression;
				if (pipe != null)
				{
					ReadCommands(pipe, jObject, serializer);
				}
				else
				{
					ReadExpressions(result as ExpressionBase, jObject, serializer);
				}
				jObject.Remove(ExpressionBase.EXPRESSIONS_NAME);
			}
			return result;
		}

		private static void ReadCommands(PipelineExpression pipe, JObject jObject, JsonSerializer serializer)
		{
			var code = jObject[PipelineExpression.CODE_NAME]?.Value<string>();
			if (!string.IsNullOrWhiteSpace(code))
			{
				var commands = ParserBuilderHelper.ParsePipelineCode(code);
				pipe.Commands.AddRange(commands);
			}
			else
			{
				var commands = serializer.Deserialize<IPipelineCommand[]>(jObject[PipelineExpression.COMMANDS_NAME]?.CreateReader());
				if (commands != null && commands.Length > 0)
				{
					pipe.Commands.AddRange(commands);
				}
			}
			jObject.Remove(PipelineExpression.COMMANDS_NAME);
			jObject.Remove(PipelineExpression.CODE_NAME);
		}

		private static void ReadExpressions(ExpressionBase expression, JObject jObject, JsonSerializer serializer)
		{
			if (expression != null)
			{
				var expressions = serializer.Deserialize<IExpression[]>(jObject[ExpressionBase.EXPRESSIONS_NAME]?.CreateReader());
				if (expressions != null && expressions.Length > 0)
				{
					expression.Expressions.AddRange(expressions);
				}
			}
		}

		private static void ReadProperties(ComplexValue value, JObject jObject, JsonSerializer serializer)
		{
			foreach (var property in jObject.Properties())
			{
				value.Properties.Add(
					property.Name,
					serializer.Deserialize<ISchema>(property.Value.CreateReader()));
			}
		}

		private static void ReadProperties(ArrayValue value, JObject jObject, JsonSerializer serializer)
		{
			var item = jObject[nameof(value.Item)];
			if (item != null)
			{
				value.Item = serializer.Deserialize<ISchema>(item.CreateReader());
			}
		}

		private static void ReadProperties(dynamic value, JObject jObject, JsonSerializer serializer)
		{
			//By default just return value
		}
	}
}