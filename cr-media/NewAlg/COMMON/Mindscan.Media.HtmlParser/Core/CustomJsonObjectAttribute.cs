using System;

namespace Mindscan.Media.HtmlParser.Core
{
	[AttributeUsage(AttributeTargets.Class)]
	internal sealed class CustomJsonObjectAttribute : Attribute
	{
		public string Name { get; }

		public string FullName { get; }

		public CustomJsonObjectAttribute(string @namespace, string name)
		{
			Name = name;
			FullName = string.IsNullOrEmpty(@namespace) ? name
				: $"{@namespace}.{name}";
		}
	}
}