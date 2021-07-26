using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mindscan.Media.HtmlParser.Core.Schema
{
	[CustomJsonObject(SupportedNames.Schema.Namespace, SupportedNames.Schema.Array)]
	internal sealed class ArrayValue : SchemaBase
	{
		[JsonProperty(Order = 3)]
		public ISchema Item { get; set; }

		protected override JToken EvaluateSchema(ExpressionContext context, dynamic data)
		{
			var result = new JArray();
			var itemSchema = Item as SchemaBase;
			if (itemSchema != null)
			{
				if (!(data is string) && (data is ICollection || data is IEnumerable))
				{
					foreach (var item in data)
					{
						var value = itemSchema.Parse(context, item);
						if (value != null)
						{
							result.Add(value);
						}
					}
				}
				else
				{
					var value = itemSchema.Parse(context, data);
					if (value != null)
					{
						result.Add(value);
					}
				}
			}
			return result.HasValues ? result : null;
		}
	}
}