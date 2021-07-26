using System.Collections.Generic;
using Mindscan.Media.HtmlParser.Core.Helpers;

namespace Mindscan.Media.HtmlParser.Tests.Stubs
{
	internal class ListCommandStub : CommandStabBase
	{
		private int _count;
		public ListCommandStub(int count)
		{
			_count = count;
		}
		public override object Run(ParserContext context, object data)
		{
			ExecuteCount++;
			var result = new List<dynamic>();
			while (_count-- > 0)
			{
				result.Add(data);
			}
			return result.ToResult();
		}
	}
}