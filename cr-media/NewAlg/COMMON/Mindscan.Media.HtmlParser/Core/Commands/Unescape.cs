using System.Text.RegularExpressions;
using Mindscan.Media.HtmlParser.Core.Helpers;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Unescape)]
	internal class Unescape : CommandBase<string>
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

			return Regex.Unescape(data).ToResult();
		}
	}
}