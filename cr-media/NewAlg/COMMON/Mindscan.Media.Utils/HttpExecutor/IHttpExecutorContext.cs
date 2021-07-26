using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Mindscan.Media.Utils.HttpExecutor
{
	public interface IHttpExecutorContext<out T>
	{
		Action<HttpWebRequest> RequestFunc { get; }
		Func<Dictionary<string, string>> HeadersFunc { get; }
		Func<CookieContainer> CookieFunc { get; }
		Action<HttpWebResponse, CookieContainer> CookieParserFunc { get; }
		Func<string, Encoding> EncodingParserFunc { get; }
		Func<StreamReader, T> ContentParserFunc { get; }
		Func<IWebProxy> ProxyFunc { get; }
		Func<TimeSpan> TimeoutFunc { get; }
	}
}
