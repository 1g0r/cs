namespace Mindscan.Media.HtmlParser.Tests.Stubs
{
	internal class NullCommandStub : CommandStabBase
	{
		public override object Run(ParserContext context, object data)
		{
			ExecuteCount++;
			return null;
		}
	}
}