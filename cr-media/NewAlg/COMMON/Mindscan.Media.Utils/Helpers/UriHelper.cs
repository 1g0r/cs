using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;

namespace Mindscan.Media.Utils.Helpers
{
	[DebuggerStepThrough]
	public static class UriHelper
	{
		private static readonly IdnMapping Idn = new IdnMapping();

		public static string NameFor<T>(this Uri uri)
		{
			return $"{typeof(T).Name}.For.{uri.NormalizeHost()}";
		}

		public static Uri Normalize(this Uri url)
		{
			return new UriBuilder
			{
				Scheme = "http",
				Host = url.NormalizeHost(),
				Path = url.NormalizePath(),
				Query = url.NormalizeQuery(),
				Fragment = url.NormalizeFragment()
			}.Uri;
		}

		private static string NormalizeHost(this Uri url)
		{
			string prefix;
			return NormalizeHost(url, out prefix);
		}

		private static string NormalizeHost(Uri url, out string prefix)
		{
			prefix = string.Empty;
			var host = url.DnsSafeHost.ToLower();
			if (host.StartsWith("www."))
			{
				host = host.Remove(0, 4);
				prefix = "www.";
			}
			if (host.Contains("xn--"))
			{
				host = Idn.GetUnicode(host);
			}
			return host;
		}

		private static string NormalizePath(this Uri url)
		{
			var path = url.AbsolutePath.Trim().TrimEnd('/');
			if (path.IsNullOrEmpty())
			{
				return string.Empty;
			}
			return WebUtility.UrlDecode(path);
		}

		private static string NormalizeQuery(this Uri url)
		{
			var query = url.Query.Trim();
			if (query.IsNullOrEmpty())
			{
				return string.Empty;
			}
			return WebUtility.UrlDecode(query);
		}

		private static string NormalizeFragment(this Uri url)
		{
			var fragment = url.Fragment.Trim();
			if (fragment.IsNullOrEmpty())
			{
				return string.Empty;
			}
			return WebUtility.UrlDecode(fragment);
		}
	}
}