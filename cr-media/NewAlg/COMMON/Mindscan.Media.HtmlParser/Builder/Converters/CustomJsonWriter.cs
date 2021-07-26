using System;
using System.Collections.Generic;
using Mindscan.Media.HtmlParser.Core.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mindscan.Media.HtmlParser.Builder.Converters
{
	internal sealed class CustomJsonWriter : JsonConverter
	{
		private readonly bool _buildCode;
		public CustomJsonWriter(bool buildCode)
		{
			_buildCode = buildCode;
		}
		public override bool CanRead => false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is ComplexValue)
			{
				WriteComplexValueToJson(writer, value, serializer);
			}
			if (value is ArrayValue)
			{
				WriteArrayValueToJson(writer, value, serializer);
			}
			if (value is SimpleValue)
			{
				ReadObject(value).WriteTo(writer, this);
			}
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(ISchema).IsAssignableFrom(objectType);
		}

		private void WriteComplexValueToJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var complex = value as ComplexValue;
			if (complex != null)
			{
				var jo = ReadObject(value);
				foreach (var property in complex.Properties)
				{
					jo.Add(property.Key, JToken.FromObject(property.Value, serializer));
				}
				jo.WriteTo(writer);
			}
		}

		private void WriteArrayValueToJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var array = value as ArrayValue;
			if (array != null)
			{
				var jo = ReadObject(value);
				jo[nameof(array.Item)] = JToken.FromObject(array.Item, serializer);
				jo.WriteTo(writer);
			}
		}

		private JObject ReadObject(object value)
		{
			var result = JObject.FromObject(value, JsonSerializer.Create(new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
				Converters = new List<JsonConverter>
				{
					new ExpressionJsonWriter(_buildCode)
				}
			}));
			return result;
		}
	}
}
