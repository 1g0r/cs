using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mindscan.Media.HtmlParser.Core.Schema
{
	[CustomJsonObject(SupportedNames.Schema.Namespace, SupportedNames.Schema.Complex)]
	internal sealed class ComplexValue : SchemaBase
	{
		[JsonIgnore]
		public Dictionary<string, ISchema> Properties { get; } = new Dictionary<string, ISchema>();

		protected override JToken EvaluateSchema(ExpressionContext context, object data)
		{
			var result = new JObject();
			foreach (var propertyName in Properties.Keys)
			{
				var structure = Properties[propertyName] as SchemaBase;
				var val = structure?.Parse(context, data);
				if (val != null)
				{
					result.Add(propertyName, val);
				}
			}
			return result.HasValues ? result : null;
		}
	}
}