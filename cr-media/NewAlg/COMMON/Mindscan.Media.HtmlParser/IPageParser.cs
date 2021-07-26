using System;
using System.IO;

namespace Mindscan.Media.HtmlParser
{
	public interface IPageParser
	{
		string ParsePage(Stream content, Uri pageUrl, bool indented, bool debug);
		string ParsePage(string content, Uri pageUri, bool indented, bool debug);
		string ToJson(bool buildCode, bool indented);
	}
}