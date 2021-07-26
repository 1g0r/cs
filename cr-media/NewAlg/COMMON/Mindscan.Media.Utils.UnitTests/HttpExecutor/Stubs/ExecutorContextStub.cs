using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Mindscan.Media.Utils.HttpExecutor;

namespace Mindscan.Media.Utils.UnitTests.HttpExecutor.Stubs
{
	internal class ExecutorContextStub: IHttpExecutorContext<string>, IHttpExecutorConfigurator<string>
	{
		public bool RequestFuncWasCalled { get; private set; }
		public Action<HttpWebRequest> RequestFunc
		{
			get
			{
				return request =>
				{
					RequestFuncWasCalled = true;
				};
			}
		}

		public bool HeadersFuncWasCalled { get; private set; }

		public Func<Dictionary<string, string>> HeadersFunc
		{
			get
			{
				return () =>
				{
					HeadersFuncWasCalled = true;
					return null;
				};
			}
		}

		public bool CookieFuncWasCalled { get; private set; }

		public Func<CookieContainer> CookieFunc
		{
			get
			{
				return () =>
				{
					CookieFuncWasCalled = true;
					return null;
				};
			}
		}

		public bool CookieParserFuncWasCalled { get; private set; }

		public Action<HttpWebResponse, CookieContainer> CookieParserFunc
		{
			get
			{
				return (response, container) =>
				{
					CookieParserFuncWasCalled = true;
				};
			}
		}

		public bool EncodingParserFuncWasCalled { get; private set; }

		public Func<string, Encoding> EncodingParserFunc
		{
			get
			{
				return s =>
				{
					EncodingParserFuncWasCalled = true;
					return null;
				};
			}
		}

		public bool ContentParserFuncWasCalled { get; private set; }
		public Func<StreamReader, string> ContentParserFunc
		{
			get
			{
				return reader =>
				{
					ContentParserFuncWasCalled = true;
					return reader.ReadToEnd();
				};
			}
		}

		public bool ProxyFuncWasCalled { get; private set; }

		public Func<IWebProxy> ProxyFunc
		{
			get
			{
				return () =>
				{
					ProxyFuncWasCalled = true;
					return null;
				};
			}
		}

		public bool TimeoutFunkWasCalled { get; private set; }
		public Func<TimeSpan> TimeoutFunc
		{
			get
			{
				return () =>
				{
					TimeoutFunkWasCalled = true;
					return TimeSpan.Zero;
				};
			}
		}

		IHttpExecutorConfigurator<string> IHttpExecutorConfigurator<string>.UseHeaders(Func<Dictionary<string, string>> headersFunc)
		{
			throw new NotImplementedException();
		}

		IHttpExecutorConfigurator<string> IHttpExecutorConfigurator<string>.UseCookie(Func<CookieContainer> cookieFunc)
		{
			throw new NotImplementedException();
		}

		IHttpExecutorConfigurator<string> IHttpExecutorConfigurator<string>.UseCookieParser(Action<HttpWebResponse, CookieContainer> cookieParserFunc)
		{
			throw new NotImplementedException();
		}

		IHttpExecutorConfigurator<string> IHttpExecutorConfigurator<string>.UseEncodingParser(Func<string, Encoding> encodingFunc)
		{
			throw new NotImplementedException();
		}

		IHttpExecutorConfigurator<string> IHttpExecutorConfigurator<string>.UseContentParser(Func<StreamReader, string> contentParserFunc)
		{
			throw new NotImplementedException();
		}

		IHttpExecutorConfigurator<string> IHttpExecutorConfigurator<string>.UseProxy(Func<IWebProxy> proxyFunc)
		{
			throw new NotImplementedException();
		}

		public IHttpExecutorConfigurator<string> UseTimeout(Func<TimeSpan> timeoutFunc)
		{
			throw new NotImplementedException();
		}

		public IHttpExecutorContext<string> BuildContext()
		{
			return this;
		}

		IHttpExecutorConfigurator<string> IHttpExecutorConfigurator<string>.UseRequest(Action<HttpWebRequest> requestFunc)
		{
			throw new NotImplementedException();
		}
	}
}