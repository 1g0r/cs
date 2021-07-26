using System;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.ToFakeUrl)]
	internal class ToFakeUrl : CommandBase<string>
	{
		protected internal override void FromExpression(string json)
		{
			
		}

		protected internal override string BuildParametersJson()
		{
			return string.Empty;
		}

		protected override object Execute(ParserContext context, string data)
		{
			if (string.IsNullOrWhiteSpace(data))
				return null;

			var builder = new UriBuilder(context.PageUrl)
			{
				Fragment = data.GetHashCode().ToString()
			};
			return builder.Uri;
		}
	}
}