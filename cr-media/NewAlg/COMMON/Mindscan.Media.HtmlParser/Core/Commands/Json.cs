using Newtonsoft.Json.Linq;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Json)]
	internal class Json : CommandBase<string>
	{
		protected internal override void FromExpression(string json)
		{

		}

		protected internal override string BuildParametersJson()
		{
			return "";
		}

		protected override object Execute(ParserContext context, string value)
		{
			// Should not pass through any terminal values
			if (!string.IsNullOrWhiteSpace(value))
			{
				return JToken.Parse(value);
			}
			return null;
		}
	}
}