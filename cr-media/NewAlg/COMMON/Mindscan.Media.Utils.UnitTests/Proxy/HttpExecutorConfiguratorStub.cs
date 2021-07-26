using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Mindscan.Media.Utils.HttpExecutor;

namespace Mindscan.Media.Utils.UnitTests.Proxy
{
	internal class HttpExecutorConfiguratorStub<T> : IHttpExecutorConfigurator<T>
	{
		public IHttpExecutorConfigurator<T> UseRequest(Action<HttpWebRequest> requestFunc)
		{
			return this;
		}

		public IHttpExecutorConfigurator<T> UseHeaders(Func<Dictionary<string, string>> headersFunc)
		{
			return this;
		}

		public IHttpExecutorConfigurator<T> UseCookie(Func<CookieContainer> cookieFunc)
		{
			return this;
		}

		public IHttpExecutorConfigurator<T> UseCookieParser(Action<HttpWebResponse, CookieContainer> cookieParserFunc)
		{
			return this;
		}

		public IHttpExecutorConfigurator<T> UseEncodingParser(Func<string, Encoding> encodingFunc)
		{
			return this;
		}

		public IHttpExecutorConfigurator<T> UseContentParser(Func<StreamReader, T> contentParserFunc)
		{
			return this;
		}

		public IHttpExecutorConfigurator<T> UseProxy(Func<IWebProxy> proxyFunc)
		{
			return this;
		}

		public IHttpExecutorConfigurator<T> UseTimeout(Func<TimeSpan> timeoutFunc)
		{
			return this;
		}

		public IHttpExecutorContext<T> BuildContext()
		{
			throw new NotImplementedException();
		}
	}
}