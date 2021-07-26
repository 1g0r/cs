namespace Mindscan.Media.HtmlParser
{
	public interface IPipelineCommand
	{
		object Run(ParserContext context, object data);
	}
}