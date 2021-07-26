using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Mime;
using System.Text;

namespace Mindscan.Media.Utils.HttpExecutor.Impl
{
	internal abstract class HttpExecutorBase
	{
		// By default request timeout equals to 100 000 ms that is too match.
		private const int DefaultTimeoutMilliseconds = 30000;

		public IHttpExecutorConfigurator<string> CreateConfigurator()
		{
			return new HttpExecutorContext<string>().UseContentParser(reader => reader.ReadToEnd());
		}

		public IHttpExecutorConfigurator<TResult> CreateConfigurator<TResult>()
		{
			return new HttpExecutorContext<TResult>();
		}

		protected IHttpResponse<T> SendMessage<T>(Uri uri, HttpMethod method, IHttpExecutorContext<T> context, Dictionary<string, string> postParameters = null, string body = null)
		{
			HttpWebRequest request = null;
			try
			{
				request = CreateRequest(uri, method, context);
				var cookies = AddCookie(request, context);

				if (request.Method == HttpMethod.Post.Method)
				{
					if (postParameters != null)
					{
						SetContent(request, postParameters);
					}
					else if (body != null)
					{
						SetContent(request, body);
					}
				}
				return GetResponse(request, context, cookies);
			}
			catch (Exception ex)
			{
				throw new HttpExecutorException(GetErrorMessage(request, uri), ex);
			}

		}

		protected IHttpResponse SendMessageNoBody(Uri uri, IHttpExecutorContext<string> context)
		{
			HttpWebRequest request = null;
			try
			{
				request = CreateRequest(uri, HttpMethod.Head, context);
				var cookies = AddCookie(request, context);

				return GetResponseNoBody(request, context, cookies);
			}
			catch (Exception ex)
			{
				throw new HttpExecutorException(GetErrorMessage(request, uri), ex);
			}
		}

		private HttpWebRequest CreateRequest<T>(Uri uri, HttpMethod method, IHttpExecutorContext<T> context)
		{
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Ssl3 |
				SecurityProtocolType.Tls |
				SecurityProtocolType.Tls11 |
				SecurityProtocolType.Tls12;

			var request = WebRequest.CreateHttp(uri);

			request.Method = method.Method;
			request.AllowAutoRedirect = true;
			request.MaximumAutomaticRedirections = 5;
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			request.UserAgent =
				"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36";
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
			request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
			request.Headers.Add("Pragma", "no-cache");
			SetTimeout(request, context);

			request.Proxy = context.ProxyFunc?.Invoke() ?? WebRequest.DefaultWebProxy;
			context.RequestFunc?.Invoke(request);

			AddHeaders(request, context);

			return request;
		}

		private static void SetTimeout<T>(HttpWebRequest request, IHttpExecutorContext<T> context)
		{
			var result = DefaultTimeoutMilliseconds;
			if (context.TimeoutFunc != null)
			{
				var timeout = context.TimeoutFunc();
				if (timeout != TimeSpan.Zero)
				{
					result = (int)timeout.TotalMilliseconds;
				}
			}
			request.Timeout = result;
		}

		private static void AddHeaders<T>(HttpWebRequest request, IHttpExecutorContext<T> context)
		{
			var customHeaders = context.HeadersFunc?.Invoke();
			if (customHeaders != null)
			{
				SetHeaders(request, customHeaders);
			}
		}

		private static void SetHeaders(HttpWebRequest request, Dictionary<string, string> headers)
		{
			foreach (var key in headers.Keys)
			{
				switch (key.ToLower())
				{
					case "accept":
						request.Accept = headers[key];
						break;
					case "user-agent":
					case "useragent":
						request.UserAgent = headers[key];
						break;
					case "host":
						request.Host = headers[key];
						break;
					case "referer":
						request.Referer = headers[key];
						break;
					case "content-type":
						request.ContentType = headers[key];
						break;
					case "connection":
						SetConnection(request, headers[key]);
						break;
					default:
						request.Headers.Add(key, headers[key]);
						break;
				}
			}
		}

		private static void SetConnection(HttpWebRequest request, string value)
		{
			switch (value.ToLower())
			{
				case "keep-alive":
					request.KeepAlive = true;
					break;
				default:
					request.Connection = value;
					break;
			}
		}

		private CookieContainer AddCookie<T>(HttpWebRequest request, IHttpExecutorContext<T> context)
		{
			var cookiesJar = context.CookieFunc?.Invoke();
			if (cookiesJar != null)
			{
				request.CookieContainer = cookiesJar;
			}
			return cookiesJar;
		}

		//For unit tests only
		protected virtual void SetContent(HttpWebRequest request, Dictionary<string, string> parameters)
		{
			request.ContentType = "application/x-www-form-urlencoded";
			if (parameters != null)
			{
				using (var t = new FormUrlEncodedContent(parameters))
				using (var reader = t.ReadAsStreamAsync().Result)
				{
					request.ContentLength = reader.Length;
					using (var writer = request.GetRequestStream())
					{
						reader.CopyTo(writer);
						writer.Close();
					}
				}
			}
		}

		//For unit tests only
		protected virtual void SetContent(HttpWebRequest request, string body)
		{
			using (var streamWriter = new StreamWriter(request.GetRequestStream()))
			{
				streamWriter.Write(body);
			}
		}

		// For unit test only
		protected virtual IHttpResponse<T> GetResponse<T>(HttpWebRequest request, IHttpExecutorContext<T> context, CookieContainer cookies)
		{
			using (var response = (HttpWebResponse)request.GetResponse())
			{
				return ParseCookie(response, context, cookies);
			}
		}

		private IHttpResponse<T> ParseCookie<T>(HttpWebResponse response, IHttpExecutorContext<T> context, CookieContainer cookies)
		{
			context.CookieParserFunc?.Invoke(response, cookies);
			return GetEncoding(response, context);
		}

		private IHttpResponse<T> GetEncoding<T>(HttpWebResponse response, IHttpExecutorContext<T> context)
		{
			var charSet = GetCharSet(response.ContentType);
			var encoding = context.EncodingParserFunc?.Invoke(charSet) ?? ParseEncoding(charSet);
			return Parse(response, encoding, context);
		}

		private static string GetCharSet(string contentType)
		{
			try
			{
				if (!string.IsNullOrEmpty(contentType))
				{
					var result = new ContentType(contentType);
					return result.CharSet;
				}
			}
			catch
			{

			}
			return string.Empty;
		}

		private static Encoding ParseEncoding(string charSet)
		{
			if (!string.IsNullOrEmpty(charSet))
			{
				var lower = charSet.ToLower(CultureInfo.InvariantCulture);
				if (lower.Contains("1251"))
				{
					return Encoding.GetEncoding("windows-1251");
				}

				return TryParseEncoding(lower);
			}
			return Encoding.UTF8;
		}

		private static Encoding TryParseEncoding(string name)
		{
			try
			{
				return Encoding.GetEncoding(name);
			}
			catch
			{
				return Encoding.UTF8;
			}
		}

		private IHttpResponse<T> Parse<T>(HttpWebResponse response, Encoding encoding, IHttpExecutorContext<T> context)
		{
			using (var stream = response.GetResponseStream())
			{
				if (stream == null)
					throw new WebException($"Unable to read response stream for uri='{response.ResponseUri}'");
				using (var reader = new StreamReader(stream, encoding))
				{
					var content = context.ContentParserFunc != null ? context.ContentParserFunc(reader) : default(T);
					return new HttpResponse<T>
					{
						Content = content,
						StatusCode = response.StatusCode,
						RedirectedUri = response.ResponseUri
					};
				}
			}
		}

		private static string GetErrorMessage(HttpWebRequest request, Uri url)
		{
			var proxy = request?.Proxy?.GetProxy(url);
			var result = $"Unable to load the page '{url}'";
			if (proxy != null && proxy != url)
			{
				result += $" through proxy '{proxy}'";
			}
			return result + $". For details see inner exception.";
		}

		//For unit tests only
		protected virtual IHttpResponse GetResponseNoBody<T>(HttpWebRequest request, IHttpExecutorContext<T> context, CookieContainer cookies)
		{
			using (var response = (HttpWebResponse)request.GetResponse())
			{
				context.CookieParserFunc?.Invoke(response, cookies);
				return new HttpResponse
				{
					RedirectedUri = response.ResponseUri,
					StatusCode = response.StatusCode
				};
			}
		}

		#region Responses
		private class HttpResponse<T> : IHttpResponse<T>
		{
			public HttpStatusCode StatusCode { get; set; }
			public Uri RedirectedUri { get; set; }
			public T Content { get; set; }
		}

		private class HttpResponse : IHttpResponse
		{
			public HttpStatusCode StatusCode { get; set; }
			public Uri RedirectedUri { get; set; }
		}
		#endregion
	}
}
