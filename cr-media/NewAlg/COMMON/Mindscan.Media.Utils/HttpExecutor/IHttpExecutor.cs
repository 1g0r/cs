using System;
using System.Collections.Generic;

namespace Mindscan.Media.Utils.HttpExecutor
{
	public interface IHttpExecutor
	{
		IHttpExecutorConfigurator<string> CreateConfigurator();
		IHttpExecutorConfigurator<TResult> CreateConfigurator<TResult>();
		IHttpResponse<string> Get(Uri uri);
		IHttpResponse<string> Get(Uri uri, IHttpExecutorConfigurator<string> configurator);
		IHttpResponse<T> Get<T>(Uri uri, IHttpExecutorConfigurator<T> configurator);
		IHttpResponse<string> Post(Uri uri, Dictionary<string, string> postParameters);
		IHttpResponse<string> Post(Uri uri, Dictionary<string, string> postParameters, IHttpExecutorConfigurator<string> configurator);
		IHttpResponse<T> Post<T>(Uri uri, Dictionary<string, string> postParameters, IHttpExecutorConfigurator<T> configurator);
		IHttpResponse<string> Post(Uri uri, string body);
		IHttpResponse<string> Post(Uri uri, string body, IHttpExecutorConfigurator<string> configurator);
		IHttpResponse<T> Post<T>(Uri uri, string body, IHttpExecutorConfigurator<T> configurator);
		IHttpResponse Head(Uri uri);
		IHttpResponse Head(Uri uri, IHttpExecutorConfigurator<string> configurator);
	}
}
