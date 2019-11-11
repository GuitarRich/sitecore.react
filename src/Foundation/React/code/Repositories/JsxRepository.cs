using System.Collections.Generic;
using Sitecore.Data;

namespace Foundation.React.Repositories
{
	public class JsxRepository
	{
		private static JsxRepository _current;

		private readonly List<string> _items = new List<string>();
		private readonly List<ID> _seenRenderings = new List<ID>();

		public static JsxRepository Current => _current ?? (_current = new JsxRepository());

		public IEnumerable<string> Items => _items;

		public void AddScript(string file, ID renderingId)
		{
			lock (_items)
			{
				if (!_seenRenderings.Contains(renderingId))
				{
					_seenRenderings.Add(renderingId);
					_items.Add(file);
				}
			}
		}
	}
}
