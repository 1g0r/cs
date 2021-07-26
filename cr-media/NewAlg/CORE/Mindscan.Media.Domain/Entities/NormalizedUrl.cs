using System;

namespace Mindscan.Media.Domain.Entities
{
	public sealed class NormalizedUrl
	{
		private NormalizedUrl() { }
		public string Prefix { get; internal set; }
		public string Tail { get; internal set; }
		public Uri Value { get; internal set; }
		public string Host { get; internal set; }
		public string LocalPath => Value.LocalPath;

		public static NormalizedUrl Build(Uri value)
		{
			return new NormalizedUrlBuilder(new NormalizedUrl()).Build(value);
		}

		internal static NormalizedUrl Build(string value)
		{
			return new NormalizedUrlBuilder(new NormalizedUrl()).Build(value);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(NormalizedUrl))
				return false;

			return Equals((NormalizedUrl)obj);
		}

		public override int GetHashCode()
		{
			return Tail.GetHashCode();
		}

		public static bool operator == (NormalizedUrl a, NormalizedUrl b)
		{
			if (ReferenceEquals(a, null))
			{
				if (ReferenceEquals(b, null))
				{
					return true;
				}

				return false;
			}

			return a.Equals(b);
		}

		public static bool operator !=(NormalizedUrl a, NormalizedUrl b)
		{
			return !(a == b);
		}

		public bool Equals(NormalizedUrl nUrl)
		{
			return Prefix == nUrl?.Prefix && Tail == nUrl?.Tail;
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}