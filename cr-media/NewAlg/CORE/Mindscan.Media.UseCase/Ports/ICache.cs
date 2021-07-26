using System.Collections.Generic;

namespace Mindscan.Media.UseCase.Ports
{
	public interface ICache<TItem>
	{
		bool Contains(string key, out TItem item);
		void Add(IEnumerable<TItem> items);
		void Add(string key, TItem item);
		TItem Get(string key);
	}
}