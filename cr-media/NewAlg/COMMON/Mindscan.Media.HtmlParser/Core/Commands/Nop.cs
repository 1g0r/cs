using System.Collections.Generic;
using System.Xml.Linq;
using AngleSharp.Dom;
using Newtonsoft.Json.Linq;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Nop)]
	internal class Nop: CommandBase<string, IElement, XElement, JToken, 
		IEnumerable<string>, IEnumerable<IElement>, IEnumerable<XElement>, IEnumerable<JToken>>
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
			return data;
		}

		protected override object Execute(ParserContext context, IEnumerable<string> data)
		{
			return data;
		}

		protected override object Execute(ParserContext context, IElement data)
		{
			return data;
		}

		protected override object Execute(ParserContext context, XElement data)
		{
			return data;
		}

		protected override object Execute(ParserContext context, JToken data)
		{
			return data;
		}

		protected override object Execute(ParserContext context, IEnumerable<IElement> data)
		{
			return data;
		}

		protected override object Execute(ParserContext context, IEnumerable<XElement> data)
		{
			return data;
		}

		protected override object Execute(ParserContext context, IEnumerable<JToken> data)
		{
			return data;
		}
	}
}