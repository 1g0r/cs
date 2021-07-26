namespace Mindscan.Media.HtmlParser.Tests.Stubs
{
	internal class PassThroughCommandStub: CommandStabBase
	{
		public override object Run(ParserContext context, object data)
		{
			ExecuteCount++;
			return data;
		}
	}
}