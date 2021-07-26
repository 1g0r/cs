using System;
using System.Collections.Generic;
using System.Linq;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Join)]
	internal class Join : CommandBase<IEnumerable<string>>
	{
		[JsonProperty(Order = 1)]
		public string Delimiter { get; set; }

		protected internal override void FromExpression(string json)
		{
			Delimiter = JsonConvert.DeserializeObject<List<string>>(json)?.FirstOrDefault();
		}

		protected internal override string BuildParametersJson()
		{
			return Delimiter.ToJsonParameters();
		}

		protected override object Execute(ParserContext context, IEnumerable<string> values)
		{
			var delimiter = string.IsNullOrEmpty(Delimiter) ? Environment.NewLine : Delimiter;

			return string.Join(
				delimiter, 
				values.Where(x => !string.IsNullOrEmpty(x))
			).ToResult();
		}
	}
}