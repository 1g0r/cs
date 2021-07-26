using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Mindscan.Media.Utils.HttpExecutor.Impl
{
	internal class HttpExecutorContext<T> : IHttpExecutorContext<T>, IHttpExecutorConfigurator<T>
	{
		public Action<HttpWebRequest> RequestFunc { get; private set; }
		public Func<Dictionary<string, string>> HeadersFunc { get; private set; }
		public Func<CookieContainer> CookieFunc { get; private set; }
		public Action<HttpWebResponse, CookieContainer> CookieParserFunc { get; private set; }
		public Func<string, Encoding> EncodingParserFunc { get; private set; }
		public Func<StreamReader, T> ContentParserFunc { get; private set; }
		public Func<IWebProxy> ProxyFunc { get; private set; }
		public Func<TimeSpan> TimeoutFunc { get; private set; }

		public IHttpExecutorConfigurator<T> UseRequest(Action<HttpWebRequest> requestFunc)
		{
			RequestFunc = requestFunc;
			return this;
		}

		public IHttpExecutorConfigurator<T> UseHeaders(Func<Dictionary<string, string>> headersFunc)
		{
			HeadersFunc = headersFunc;
			return this;
		}

		public IHttpExecutorConfigurator<T> UseCookie(Func<CookieContainer> cookieFunc)
		{
			CookieFunc = cookieFunc;
			return this;
		}

		public IHttpExecutorConfigurator<T> UseCookieParser(Action<HttpWebResponse, CookieContainer> cookieParserFunc)
		{
			CookieParserFunc = cookieParserFunc;
			return this;
		}

		public IHttpExecutorConfigurator<T> UseEncodingParser(Func<string, Encoding> encodingFunc)
		{
			EncodingParserFunc = encodingFunc;
			return this;
		}

		public IHttpExecutorConfigurator<T> UseContentParser(Func<StreamReader, T> contentParserFunc)
		{
			ContentParserFunc = contentParserFunc;
			return this;
		}

		public IHttpExecutorConfigurator<T> UseProxy(Func<IWebProxy> proxyFunc)
		{
			ProxyFunc = proxyFunc;
			return this;
		}

		public IHttpExecutorConfigurator<T> UseTimeout(Func<TimeSpan> timeoutFunc)
		{
			TimeoutFunc = timeoutFunc;
			return this;
		}

		public IHttpExecutorContext<T> BuildContext()
		{
			return this;
		}
	}
}
