using System;
using System.Collections.Generic;

namespace Mindscan.Media.HtmlParser
{
	public sealed class ExpressionContext: ParserContext, IDisposable
	{
		private readonly HashSet<IDisposable> _disposables = new HashSet<IDisposable>();

		internal ExpressionContext(ParserContext context):base(context.PageUrl, context.Debug)
		{
			
		}

		public void Dispose()
		{
			if (_disposables.Count > 0)
			{
				foreach (var disposable in _disposables)
				{
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		public void AddIfDisposable(dynamic value)
		{
			if (value != null && value is IDisposable)
			{
				_disposables.Add(value);
			}
		}
	}
}