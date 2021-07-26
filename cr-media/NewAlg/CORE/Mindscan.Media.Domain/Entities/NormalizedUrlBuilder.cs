using System;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Mindscan.Media.Domain.Entities
{
	internal sealed class NormalizedUrlBuilder
	{
		private static readonly IdnMapping Idn = new IdnMapping();
		private readonly NormalizedUrl _value;

		internal NormalizedUrlBuilder(NormalizedUrl value)
		{
			_value = value;
		}

		public NormalizedUrl Build(Uri url)
		{
			if(url == null)
				throw new ArgumentNullException(nameof(url));

			string prefix;
			 _value.Tail = NormalizeTail(url, out prefix);
			_value.Prefix = prefix;
			_value.Value = new Uri($"{prefix}{_value.Tail}");
			return _value;
		}

		public NormalizedUrl Build(string url)
		{
			if(string.IsNullOrWhiteSpace(url))
				throw new ArgumentNullException(nameof(url));

			string lower = url.Trim();
			if (lower.StartsWith("//"))
			{
				lower = "http:" + lower;
			}
			else if (!lower.StartsWith("http://") && !lower.StartsWith("https://"))
			{
				lower = "http://" + lower;
			}
			return Build(new Uri(lower));
		}

		private string NormalizeTail(Uri url, out string prefix)
		{
			_value.Host = NormalizeHost(url, out prefix);
			var query = NormalizeQuery(url);
			var path = NormalizePath(url);
			if (ShouldAddSlash(path, query))
			{
				path += "/";
			}
			var result = $"{_value.Host}{path}{query}{NormalizeFragment(url)}";
			return result;
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
			prefix = $"{url.Scheme}://{prefix}";
			return host;
		}

		private static string NormalizePath(Uri url)
		{
			var path = url.AbsolutePath.Trim();
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}
			return WebUtility.UrlDecode(path);
		}

		private static string NormalizeQuery(Uri url)
		{
			var query = url.Query.Trim();
			if (string.IsNullOrEmpty(query))
			{
				return string.Empty;
			}
			return WebUtility.UrlDecode(query);
		}

		private static string NormalizeFragment(Uri url)
		{
			var fragment = url.Fragment.Trim();
			if (string.IsNullOrEmpty(fragment))
			{return string.Empty;
			}
			return WebUtility.UrlDecode(fragment);
		}

		private static bool ShouldAddSlash(string path, string query)
		{
			return string.IsNullOrEmpty(path);
				//|| !IsFile(path) && !path.EndsWith("/") && string.IsNullOrEmpty(query);
		}

		private static bool IsFile(string path)
		{
			return path.Split('/').LastOrDefault()?.Contains(".") ?? false;
		}
	}
}