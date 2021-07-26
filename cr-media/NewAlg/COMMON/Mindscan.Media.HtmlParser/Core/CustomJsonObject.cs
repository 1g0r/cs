using System;
using System.Reflection;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core
{
	internal abstract class CustomJsonObject
	{
		public const string TYPE_NAME = "$type";

		private readonly Lazy<CustomJsonObjectAttribute> _attribute;

		protected CustomJsonObject()
		{
			_attribute = new Lazy<CustomJsonObjectAttribute>(() => GetType().GetCustomAttribute<CustomJsonObjectAttribute>(true));
		}

		[JsonProperty(TYPE_NAME, Order = 0)]
		public string Type => _attribute.Value.FullName;

		[JsonIgnore]
		public string Name => _attribute.Value.Name;
	}
}
