using System;
using System.Net;

namespace Mindscan.Media.Utils.HttpExecutor
{
	public interface IHttpResponse
	{
		HttpStatusCode StatusCode { get; }
		Uri RedirectedUri { get; }
	}

	public interface IHttpResponse<out T> : IHttpResponse
	{
		T Content { get; }
	}
}
