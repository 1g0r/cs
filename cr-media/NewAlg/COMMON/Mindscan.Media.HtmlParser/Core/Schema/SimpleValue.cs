using System;
using System.Collections;
using System.Numerics;
using Newtonsoft.Json.Linq;

namespace Mindscan.Media.HtmlParser.Core.Schema
{
	[CustomJsonObject(SupportedNames.Schema.Namespace, SupportedNames.Schema.Simple)]
	internal sealed class SimpleValue : SchemaBase
	{
		protected override JToken EvaluateSchema(ExpressionContext context, dynamic value)
		{
			if (context.Debug && !(value is string) && (value is ICollection || value is IEnumerable))
			{
				var result = new JArray();
				foreach (var item in value)
				{
					if (IsJValueType(item))
					{
						result.Add(item);
					}
				}
				return result;
			}
			return IsJValueType(value) ? new JValue(value) : null;
		}

		private bool IsJValueType(object value)
		{
			if (value == null || value == DBNull.Value)
				return false;
			if (value is string)
				return true;
			if (value is long || value is int || (value is short || value is sbyte) || (value is ulong || value is uint || (value is ushort || value is byte)) || (value is Enum || value is BigInteger))
				return true;
			if (value is double || value is float || value is Decimal)
				return true;
			if (value is DateTime || value is DateTimeOffset)
				return true;
			if (value is byte[])
				return true;
			if (value is bool)
				return true;
			if (value is Guid)
				return true;
			if ((object)(value as Uri) != null)
				return true;
			if (value is TimeSpan)
				return true;
			return false;
		}
	}
}