using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace Mindscan.Media.Utils.UnitTests.HttpExecutor.Stubs
{
	public class HttpWebResponseStub : HttpWebResponse
	{
		private Uri _responseUri;
		[Obsolete("", false)]
		public HttpWebResponseStub(HttpStatusCode statusCode)
		{
			StatusCode = statusCode;
		}
		protected HttpWebResponseStub(SerializationInfo serializationInfo, StreamingContext streamingContext) 
			: base(serializationInfo, streamingContext)
		{
		}

		public override string ContentType { get; set; }

		public override Stream GetResponseStream()
		{
			if (Parameters == null || Parameters.Count == 0)
			{
				return new MemoryStream(BodyEncoding.GetBytes(Body));
			}
			Body += "{ ";
			foreach (var parameter in Parameters)
			{
				Body += $"{parameter.Key}:{parameter.Value} ";
			}
			Body += "}";
			return new MemoryStream(BodyEncoding.GetBytes(Body));
		}

		public override HttpStatusCode StatusCode { get; }

		public override Uri ResponseUri => _responseUri;

		public void SetResponseUri(Uri uri)
		{
			if (_responseUri == null)
			{
				_responseUri = uri;
			}
		}

		public string Body { get; set; }

		public Encoding BodyEncoding { get; set; }

		public Dictionary<string, string> Parameters { get; set; }
	}
}