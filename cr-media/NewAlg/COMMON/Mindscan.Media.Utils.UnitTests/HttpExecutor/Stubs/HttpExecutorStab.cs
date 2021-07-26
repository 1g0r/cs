using System.Collections.Generic;
using System.Net;
using Mindscan.Media.Utils.HttpExecutor;
using Mindscan.Media.Utils.UnitTests.HttpExecutor.Stubs;

namespace Mindscan.Media.Utils.UnitTests.HttpExecutor.Stubs
{
	internal class HttpExecutorStab : Utils.HttpExecutor.Impl.HttpExecutor
	{
		private readonly HttpWebRequestStub _request;
		
		public HttpExecutorStab(HttpWebRequestStub requestToUse)
		{
			_request = requestToUse;
		}
		protected override IHttpResponse<T> GetResponse<T>(HttpWebRequest request, IHttpExecutorContext<T> context, CookieContainer cookies)
		{
			_request.SetRequest(request);
			return base.GetResponse(_request, context, cookies);
		}

		protected override IHttpResponse GetResponseNoBody<T>(HttpWebRequest request, IHttpExecutorContext<T> context, CookieContainer cookies)
		{
			_request.SetRequest(request);
			return base.GetResponseNoBody(_request, context, cookies);
		}

		protected override void SetContent(HttpWebRequest request, Dictionary<string, string> parameters)
		{
			_request.SetRequestParameters(parameters);
			_request.SetRequest(request);
			base.SetContent(_request, parameters);
		}
	}
}