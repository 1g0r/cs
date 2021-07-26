using Mindscan.Media.HtmlParser.Core.Helpers;

namespace Mindscan.Media.HtmlParser.Tests.Stubs
{
	internal class StringCommandStub : CommandStabBase
	{
		private readonly string _value;

		public StringCommandStub(string value)
		{
			_value = value;
		}
		public override object Run(ParserContext context, object data)
		{
			ExecuteCount++;
			return _value.ToResult();
		}
	}
}