using System.Collections.Generic;
using System.Linq;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Const)]
	internal class Const : CommandBase
	{
		[JsonProperty(Order = 1)]
		public List<string> Values { get; } = new List<string>();

		protected internal override void FromExpression(string json)
		{
			Values.AddNotEmptyItems(json);
		}

		protected internal override string BuildParametersJson()
		{
			return Values.ToJsonParameters();
		}

		public override object Run(ParserContext context, object data)
		{
			if (Values.Count > 1)
			{
				return Values.ToResult();
			}
			return Values
				.FirstOrDefault()
				.ToResult();
		}
	}
}