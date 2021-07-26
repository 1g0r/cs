using System.Net;
using Mindscan.Media.HtmlParser.Core.Helpers;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Decode)]
	internal class Decode : CommandBase<string>
	{
		protected internal override void FromExpression(string json)
		{
		}

		protected internal override string BuildParametersJson()
		{
			return "";
		}

		protected override object Execute(ParserContext context, string data)
		{
			if (string.IsNullOrWhiteSpace(data))
				return null;

			return WebUtility.HtmlDecode(data).ToResult();
		}
	}
}