using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Mindscan.Media.Utils.Config;
using Mindscan.Media.Utils.HttpExecutor;
using Mindscan.Media.Utils.ObjectPool;
using Newtonsoft.Json;

namespace Mindscan.Media.Utils.Proxy.Impl
{
	internal class ProxyLoader : IResourceLoader<ProxyWrapper>, IProxyFinder
	{
		private readonly IHttpExecutor _executor;
		private readonly IHttpExecutorConfigurator<ProxyEntity[]> _configurator;
		private readonly IUtilsConfig _config;
		public ProxyLoader(IHttpExecutor executor, IUtilsConfig config)
		{
			_executor = executor;
			_configurator = executor
				.CreateConfigurator<ProxyEntity[]>()
				.UseRequest(request => request.AllowAutoRedirect = false)
				.UseProxy(() => WebRequest.DefaultWebProxy)
				.UseTimeout(() => new TimeSpan(0, 1, 0))
				.UseContentParser(reader =>
				{
					using (var jsonReader = new JsonTextReader(reader))
					{
						return new JsonSerializer().Deserialize<ProxyEntity[]>(jsonReader);
					}
				});
			_config = config;
		}
		public IEnumerable<ProxyWrapper> LoadResources()
		{
			if (!_config.ProxyRepositoryConfig.Debug)
			{
				var list = FetchProxyList();
				if (list != null)
				{
					return list
						.Where(x => !string.IsNullOrWhiteSpace(x?.Address))
						.Select(p => new ProxyWrapper(p.Cast()));
				}
			}
			return new List<ProxyWrapper>
			{
				new ProxyWrapper(WebRequest.DefaultWebProxy)
			};
		}

		public IWebProxy FindProxyByAddress(Uri uri)
		{
			if (_config.ProxyRepositoryConfig.Debug)
				return null;
			return LoadResources().FirstOrDefault(x =>
			{
				var proxy = x.ProxyObject.GetProxy(uri);
				return uri.Equals(proxy);
			})?.ProxyObject;
		}

		public IEnumerable<IWebProxy> Find()
		{
			return LoadResources()
				.Select(x => x.ProxyObject);
		}

		protected internal virtual ProxyEntity[] FetchProxyList()
		{
			var response = _executor.Get(
				_config.ProxyRepositoryConfig.ProxyListUri,
				_configurator);

			if (response.StatusCode == HttpStatusCode.OK)
			{
				return response.Content;
			}
			return null;
		}

#region Executor Response
		public class ProxyEntity
		{
			public string Address { get; set; }
			public string User { get; set; }
			public string Password { get; set; }

			public WebProxy Cast()
			{
				var result = new WebProxy($"http://{Address}", true);
				if (!string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(Password))
				{
					result.Credentials = new NetworkCredential(User, Password);
				}
				return result;
			}
		}
#endregion
	}
}
