using AngleSharp.Dom;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.Take)]
	internal class Take : CommandBase<IEnumerable<string>, IEnumerable<IElement>, IEnumerable<XElement>, IEnumerable<JToken>>
	{
		[JsonProperty("Count", Order = 1)]
		private int _count;

		[JsonIgnore]
		public int Count
		{
			get { return _count < 1 ? 1 : _count; }
			set { _count = value < 1 ? 1 : value; }
		}

		protected internal override void FromExpression(string json)
		{
			var arr = JsonConvert.DeserializeObject<List<int>>(json);
			if (arr != null && arr.Count > 0)
			{
				Count = arr[0] < 1 ? 1 : arr[0];
			}
		}

		protected internal override string BuildParametersJson()
		{
			if (_count > 0)
			{
				return _count.ToJsonParameters();
			}
			return null;
		}

		protected override object Execute(ParserContext context, IEnumerable<string> elements)
		{
			return TakeSome(elements).ToResult();
		}

		protected override object Execute(ParserContext context, IEnumerable<IElement> elements)
		{
			return TakeSome(elements).ToResult();
		}

		protected override object Execute(ParserContext context, IEnumerable<XElement> elements)
		{
			return TakeSome(elements).ToResult();
		}

		protected override object Execute(ParserContext context, IEnumerable<JToken> elements)
		{
			return TakeSome(elements).ToResult();
		}

		private IEnumerable<T> TakeSome<T>(IEnumerable<T> elements)
		{
			var i = 0;
			foreach (var element in elements)
			{
				if (i++ < Count)
				{
					yield return element;
				}
				else
				{
					yield break;
				}
			}
		}
	}
}