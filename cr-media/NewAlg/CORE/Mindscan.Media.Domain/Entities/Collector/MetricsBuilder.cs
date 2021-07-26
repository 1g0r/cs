using System;

namespace Mindscan.Media.Domain.Entities.Collector
{
	public sealed class MetricsBuilder
	{
		private readonly Metrics _entity;
		internal MetricsBuilder(Metrics entity)
		{
			_entity = entity;
		}

		public MetricsBuilder SharesCount(int value)
		{
			if(value < 0)
				throw new ArgumentOutOfRangeException(nameof(SharesCount));
			_entity.SharesCount = value;
			return this;
		}

		public MetricsBuilder CommentsCount(int value)
		{
			if (value < 0)
				throw new ArgumentOutOfRangeException(nameof(CommentsCount));

			_entity.CommentsCount = value;
			return this;
		}

		public MetricsBuilder LikesCount(int value)
		{
			if(value < 0)
				throw new ArgumentOutOfRangeException(nameof(LikesCount));

			_entity.LikesCount = value;
			return this;
		}

		public Metrics Build()
		{
			var t = _entity.CommentsCount + _entity.LikesCount + _entity.SharesCount;
			if(t < 1)
				throw new InvalidOperationException("Specify at least one metric.");
			return _entity;
		}
	}
}