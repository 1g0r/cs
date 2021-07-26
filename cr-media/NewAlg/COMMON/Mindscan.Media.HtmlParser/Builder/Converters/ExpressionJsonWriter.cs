using System;
using Mindscan.Media.HtmlParser.Core.Expression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mindscan.Media.HtmlParser.Builder.Converters
{
	internal sealed class ExpressionJsonWriter : JsonConverter
	{
		private readonly bool _buildCode;

		public ExpressionJsonWriter(bool buildCode)
		{
			_buildCode = buildCode;
		}
		public override bool CanRead => false;

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
		public override bool CanConvert(Type objectType)
		{
			return typeof(IExpression).IsAssignableFrom(objectType);
		}
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is PipelineExpression)
			{
				WritePipelineToJson(writer, value, serializer);
			}
			else
			{
				var b = value as ExpressionBase;
				if (b != null)
				{
					var jo = ReadObject(value);
					var arr =  new JArray();
					foreach (var expression in b.Expressions)
					{
						arr.Add(JObject.FromObject(expression, serializer));
					}
					jo[ExpressionBase.EXPRESSIONS_NAME] = arr;
					jo.WriteTo(writer);
				}
			}
		}

		private void WritePipelineToJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var pipe = value as PipelineExpression;
			if (pipe != null)
			{
				var jo = ReadObject(value);
				if (_buildCode)
				{
					jo.Remove(PipelineExpression.COMMANDS_NAME);
					jo[PipelineExpression.CODE_NAME] = new JValue(ParserBuilderHelper.BuildPipelineCode(pipe.Commands));
				}
				else
				{
					jo[PipelineExpression.COMMANDS_NAME] = JArray.FromObject(pipe.Commands, serializer);
				}
				jo.Remove(ExpressionBase.EXPRESSIONS_NAME);
				jo.WriteTo(writer);
			}
		}

		private JObject ReadObject(object value)
		{
			var result = JObject.FromObject(value, JsonSerializer.Create(new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			}));
			return result;
		}

	}
}