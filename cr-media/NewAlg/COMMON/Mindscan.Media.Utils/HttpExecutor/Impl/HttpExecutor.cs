using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Mindscan.Media.Utils.HttpExecutor.Impl
{
	internal class HttpExecutor : HttpExecutorBase, IHttpExecutor
	{
		public IHttpResponse<string> Get(Uri uri)
		{
			return Get(uri, CreateConfigurator());
		}

		public IHttpResponse<string> Get(Uri uri, IHttpExecutorConfigurator<string> configurator)
		{
			if (configurator == null)
			{
				throw new ArgumentNullException(nameof(configurator), "Parameter can not be null. Use overloaded method instead.");
			}
			return SendMessage(uri, HttpMethod.Get, configurator.BuildContext());
		}

		public IHttpResponse<T> Get<T>(Uri uri, IHttpExecutorConfigurator<T> configurator)
		{
			if (configurator == null)
			{
				throw new ArgumentNullException(nameof(configurator), "Parameter can not be null. Use overloaded method instead.");
			}
			return SendMessage(uri, HttpMethod.Get, configurator.BuildContext());
		}

		public IHttpResponse<string> Post(Uri uri, Dictionary<string, string> postParameters)
		{
			return Post(uri, postParameters, CreateConfigurator());
		}

		public IHttpResponse<string> Post(Uri uri, Dictionary<string, string> postParameters, IHttpExecutorConfigurator<string> configurator)
		{
			if (configurator == null)
			{
				throw new ArgumentNullException(nameof(configurator), "Parameter can not be null. Use overloaded method instead.");
			}
			return SendMessage(uri, HttpMethod.Post, configurator.BuildContext(), postParameters);
		}

		public IHttpResponse<T> Post<T>(Uri uri, Dictionary<string, string> postParameters, IHttpExecutorConfigurator<T> configurator)
		{
			if (configurator == null)
			{
				throw new ArgumentNullException(nameof(configurator), "Parameter can not be null. Use overloaded method instead.");
			}
			return SendMessage(uri, HttpMethod.Post, configurator.BuildContext(), postParameters);
		}

		public IHttpResponse<string> Post(Uri uri, string body)
		{
			return Post(uri, body, CreateConfigurator());
		}

		public IHttpResponse<string> Post(Uri uri, string body, IHttpExecutorConfigurator<string> configurator)
		{
			if (configurator == null)
			{
				throw new ArgumentNullException(nameof(configurator), "Parameter can not be null. Use overloaded method instead.");
			}
			return SendMessage(uri, HttpMethod.Post, configurator.BuildContext(), null, body);
		}

		public IHttpResponse<T> Post<T>(Uri uri, string body, IHttpExecutorConfigurator<T> configurator)
		{
			if (configurator == null)
			{
				throw new ArgumentNullException(nameof(configurator), "Parameter can not be null. Use overloaded method instead.");
			}
			return SendMessage(uri, HttpMethod.Post, configurator.BuildContext(), null, body);
		}

		public IHttpResponse Head(Uri uri)
		{
			return Head(uri, CreateConfigurator());
		}

		public IHttpResponse Head(Uri uri, IHttpExecutorConfigurator<string> configurator)
		{
			if (configurator == null)
			{
				throw new ArgumentNullException(nameof(configurator), "Parameter can not be null. Use overloaded method instead.");
			}
			return SendMessageNoBody(uri, configurator.BuildContext());
		}
	}
}
