namespace Mindscan.Media.Domain.Entities
{
	public abstract class PagedFilter
	{
		public int Offset { get; set; }
		public int Count { get; set; }
		public bool LeadingWildcard { get; set; }
		public bool FollowingWildcard { get; set; }

		internal void SetDefaults()
		{
			if (Count > 1000 || Count < 1)
				Count = 20;

			if (Offset < 0)
				Offset = 0;
			SetCustomDefaults();
		}

		protected virtual void SetCustomDefaults()
		{

		}

		protected static string PrepareUrlPrefix(string value)
		{
			if (!string.IsNullOrWhiteSpace(value))
			{
				return NormalizedUrl
					.Build(value)
					.Tail
					.TrimEnd('/');
			}

			return null;
		}
	}
}