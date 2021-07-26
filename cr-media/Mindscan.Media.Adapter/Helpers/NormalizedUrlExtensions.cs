using System;
using System.Security.Cryptography;
using System.Text;
using Mindscan.Media.Domain.Entities;
using Mindscan.Media.Utils.Helpers;

namespace Mindscan.Media.Adapter.Helpers
{
	internal static class NormalizedUrlExtensions
	{
		public static string ComputeMd5Hash(this NormalizedUrl url)
		{
			var result = url.Tail.IsNullOrWhiteSpace() ? String.Empty : url.Tail;
			using (var enc = MD5.Create())
			{
				return ConvertToString(
					enc.ComputeHash(Encoding.Unicode.GetBytes(result))
				);
			}
		}

		private static string ConvertToString(byte[] hash)
		{
			var sb = new StringBuilder();
			foreach (var b in hash)
			{
				sb.Append(b.ToString("x2"));
			}
			return sb.ToString();
		}
	}
}