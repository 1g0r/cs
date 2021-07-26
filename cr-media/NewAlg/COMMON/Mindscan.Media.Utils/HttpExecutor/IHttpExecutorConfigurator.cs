using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Mindscan.Media.Utils.HttpExecutor
{
	public interface IHttpExecutorConfigurator<T>
	{
		IHttpExecutorConfigurator<T> UseRequest(Action<HttpWebRequest> requestFunc);
		IHttpExecutorConfigurator<T> UseHeaders(Func<Dictionary<string, string>> headersFunc);
		IHttpExecutorConfigurator<T> UseCookie(Func<CookieContainer> cookieFunc);
		IHttpExecutorConfigurator<T> UseCookieParser(Action<HttpWebResponse, CookieContainer> cookieParserFunc);
		IHttpExecutorConfigurator<T> UseEncodingParser(Func<string, Encoding> encodingFunc);
		IHttpExecutorConfigurator<T> UseContentParser(Func<StreamReader, T> contentParserFunc);
		IHttpExecutorConfigurator<T> UseProxy(Func<IWebProxy> proxyFunc);
		IHttpExecutorConfigurator<T> UseTimeout(Func<TimeSpan> timeoutFunc);
		IHttpExecutorContext<T> BuildContext();
	}
}