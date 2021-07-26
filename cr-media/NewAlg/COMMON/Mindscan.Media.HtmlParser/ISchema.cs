using Newtonsoft.Json.Linq;

namespace Mindscan.Media.HtmlParser
{
	public interface ISchema
	{
		JToken Parse(ParserContext context, object data);
	}
}