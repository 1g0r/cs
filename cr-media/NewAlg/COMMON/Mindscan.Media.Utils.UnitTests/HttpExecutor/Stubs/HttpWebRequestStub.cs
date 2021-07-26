using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;

namespace Mindscan.Media.Utils.UnitTests.HttpExecutor.Stubs
{
	public sealed class HttpWebRequestStub : HttpWebRequest
	{
		private HttpWebRequest _request;
		private readonly HttpWebResponseStub _response;

		[Obsolete("", false)]
		public HttpWebRequestStub(HttpWebResponseStub responseToUse)
		{
			ContentType = "text/html; charset=utf-8";
			_response = responseToUse;
		}

		protected HttpWebRequestStub(SerializationInfo serializationInfo, StreamingContext streamingContext) 
			: base(serializationInfo, streamingContext)
		{
		}

		public override WebResponse GetResponse()
		{
			_response.SetResponseUri(_request.RequestUri);
			// To Close connection get fake response
			try
			{
				using(var t = _request.GetResponse()) { }
			}
			catch { }
			return _response;
		}

		public void SetRequest(HttpWebRequest request)
		{
			_request = request;
		}

		public void SetRequestParameters(Dictionary<string, string> parametersDictionary)
		{
			_response.Parameters = parametersDictionary;
		}

		public override string ContentType { get; set; }

		public override Stream GetRequestStream()
		{
			return _request.GetRequestStream();
		}
	}
}