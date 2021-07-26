using System;
using System.Diagnostics;
using Mindscan.Media.Domain.Const;

namespace Mindscan.Media.Domain.Entities.Scheduler
{
	[DebuggerStepThrough]
	public class TriggerBuilder : EntityBuilderBase<Trigger, TriggerBuilder>
	{
		internal TriggerBuilder():base(new Trigger())
		{
			
		}
		
		public TriggerBuilder FireTimeUtc(DateTime value)
		{
			AssertUtc(value, nameof(FireTimeUtc));
			Entity.FireTimeUtc = value;
			return this;
		}

		public TriggerBuilder FireCount(int value)
		{
			if (value > 0)
			{
				Entity.FireCount = value;
			}
			return this;
		}

		public TriggerBuilder FeedId(long value)
		{
			Entity.FeedId = value;
			return this;
		}

		public TriggerBuilder RoutingKey(string value)
		{
			AssertRequired(value, nameof(RoutingKey));
			Entity.RoutingKey = value.Trim();
			return this;
		}

		public TriggerBuilder VirtualHost(string value)
		{
			if (!string.IsNullOrWhiteSpace(value))
			{
				Entity.VirtualHost = value.Trim();
			}

			return this;
		}

		public TriggerBuilder Enabled(bool value)
		{
			Entity.Enabled = value;

			return this;
		}

		public TriggerBuilder RepeatInterval(TimeSpan value)
		{
			if (value > TimeSpan.FromMinutes(5))
			{
				Entity.RepeatInterval = value;
			}
			else
			{
				throw new ArgumentOutOfRangeException(nameof(RepeatInterval));
			}

			return this;
		}

		public TriggerBuilder StartAtUtc(DateTime? value)
		{
			if (value.HasValue)
			{
				AssertUtc(value.Value, nameof(StartAtUtc));
				Entity.StartAtUtc = value;
			}
			return this;
		}

		public TriggerBuilder Payload(string value)
		{
			if (!string.IsNullOrWhiteSpace(value))
			{
				Entity.Payload = value;
			}

			return this;
		}

		protected override void BuildInternal()
		{
			AssertRequired(Entity.RoutingKey, nameof(RoutingKey));
			AssertRequired(Entity.RepeatInterval, nameof(RepeatInterval));
			if (string.IsNullOrWhiteSpace(Entity.VirtualHost))
			{
				Entity.VirtualHost = GlobalDefaults.VirtualHost;
			}
		}
	}
}