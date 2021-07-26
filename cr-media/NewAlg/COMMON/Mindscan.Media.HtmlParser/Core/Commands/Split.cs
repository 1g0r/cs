using System.Collections.Generic;
using System.Linq;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Split)]
	internal sealed class Split : CommandBase<string>
	{
		[JsonProperty(Order = 1)]
		public string Pattern { get; set; }
		protected internal override void FromExpression(string json)
		{
			Pattern = JsonConvert.DeserializeObject<List<string>>(json)?.FirstOrDefault();
		}

		protected internal override string BuildParametersJson()
		{
			return Pattern.ToJsonParameters();
		}

		protected override object Execute(ParserContext context, string data)
		{
			if (string.IsNullOrWhiteSpace(data))
				return null;

			foreach (var regex in new List<string>{Pattern}.ToRegex())
			{
				return regex.Split(data)
					.ToResult();
			}
			return new List<string>{data}
				.ToResult();
		}
	}
}