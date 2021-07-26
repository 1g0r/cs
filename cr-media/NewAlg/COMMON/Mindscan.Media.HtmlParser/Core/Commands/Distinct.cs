using System.Collections.Generic;
using System.Linq;
using Mindscan.Media.HtmlParser.Core.Helpers;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Distinct)]
	internal class Distinct : CommandBase<IEnumerable<string>>
	{
		protected internal override void FromExpression(string json)
		{

		}

		protected internal override string BuildParametersJson()
		{
			return "";
		}

		protected override object Execute(ParserContext context, IEnumerable<string> values)
		{
			return values
				.Distinct()
				.ToResult();
		}
	}
}