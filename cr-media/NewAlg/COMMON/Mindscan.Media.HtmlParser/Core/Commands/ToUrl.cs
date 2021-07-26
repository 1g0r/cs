using System;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.ToUrl)]
	internal class ToUrl : CommandBase<string>
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
			if (string.IsNullOrWhiteSpace(data) || data.Contains("data:image") || data.StartsWith("file:///"))
				return null;
			if (data.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
			{
				return new Uri(data);
			}
			if (data.StartsWith("//"))
			{
				return new Uri(context.PageUrl.Scheme + ":" + data);
			}

			if (data.StartsWith("/"))
			{
				var userInfo = string.IsNullOrEmpty(context.PageUrl.UserInfo) ? string.Empty : context.PageUrl.UserInfo + "@";
				return new Uri($"{context.PageUrl.Scheme}://{userInfo}{context.PageUrl.Authority}{data}");
			}
			return new Uri(context.PageUrl.ToString().TrimEnd('/') + "/" + data);
		}
	}
}