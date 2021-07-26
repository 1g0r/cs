using System.Collections.Generic;
using System.Xml.Linq;
using AngleSharp.Dom;
using Mindscan.Media.HtmlParser.Builder;
using Mindscan.Media.HtmlParser.Core.Helpers;
using Newtonsoft.Json;

namespace Mindscan.Media.HtmlParser.Core.Commands
{
	[CustomJsonObject(SupportedNames.Commands.Namespace, SupportedNames.Commands.SkipLast)]
	internal class SkipLast : CommandBase<IEnumerable<IElement>, IEnumerable<XElement>>
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

		protected override object Execute(ParserContext context, IEnumerable<IElement> elements)
		{
			return Skip(elements).ToResult();
		}

		protected override object Execute(ParserContext context, IEnumerable<XElement> values)
		{
			return Skip(values).ToResult();
		}

		private IEnumerable<T> Skip<T>(IEnumerable<T> elements)
		{
			var queue = new Queue<T>(Count);
			foreach (var element in elements)
			{
				if (queue.Count < Count)
				{
					queue.Enqueue(element);
				}
				else
				{
					yield return queue.Dequeue();
					queue.Enqueue(element);
				}
			}
		}
	}
}