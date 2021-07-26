using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Mindscan.Media.Utils.Helpers
{
	[DebuggerStepThrough]
	public static class StringHelper
	{
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> enums)
		{
			return enums == null || !enums.Any();
		}

		public static bool IsNullOrEmpty(this string value)
		{
			return string.IsNullOrEmpty(value);
		}

		public static bool IsNullOrWhiteSpace(this string value)
		{
			return string.IsNullOrWhiteSpace(value);
		}

		public static string Fill(this string format, params object[] args)
		{
			return string.Format(format, args);
		}

		public static string Convert(this int val)
		{
			return val.ToString(CultureInfo.InvariantCulture);
		}

		public static string Convert(this int? value)
		{
			return value.HasValue ? value.Value.Convert() : string.Empty;
		}

		public static bool IsAuthorName(this string maybeAuthorName)
		{
			if (maybeAuthorName.IsNullOrWhiteSpace())
				return false;

			Regex regex = new Regex(GetAuthorRegexPattern());

			return regex.Match(maybeAuthorName).Success;
		}

		public static string GetAuthorRegexPattern()
		{
			return "^\\p{Lu}\\p{L}* \\p{Lu}\\p{L}*$";
		}

		public static T FromJson<T>(this string value)
		{
			if (value.IsNullOrWhiteSpace())
				return default(T);
			return JsonConvert.DeserializeObject<T>(value);
		}

		public static string ToJson(this object value)
		{
			if (value == null)
				return null;
			return JsonConvert.SerializeObject(value);
		}
	}
}
