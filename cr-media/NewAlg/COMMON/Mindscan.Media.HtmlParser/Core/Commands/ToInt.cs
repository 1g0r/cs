namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.ToInt)]
	internal class ToInt : CommandBase<string>
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
			int result;
			if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out result))
			{
				return result;
			}
			return null;
		}
	}
}