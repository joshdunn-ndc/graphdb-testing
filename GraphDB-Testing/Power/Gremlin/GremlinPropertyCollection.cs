using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDB_Testing.Gremlin
{
	internal sealed class GremlinPropertyCollection : KeyedCollection<string, GremlinProperty>
	{
		internal GremlinPropertyCollection()
		{
		}

		internal GremlinPropertyCollection(IEnumerable<GremlinProperty> items)
		{
			if (items != null)
			{
				AddRange(items);
			}
		}

		public bool TryGetProperty(string key, out GremlinProperty property)
		{
			if (base.Dictionary != null)
			{
				return base.Dictionary.TryGetValue(key, out property);
			}
			property = null;
			return false;
		}

		internal void AddRange(IEnumerable<GremlinProperty> items)
		{
			if (items == null)
			{
				throw new Exception(string.Format("Graph Element: Property Collection: Add Range: null {0}", "items"));
			}
			foreach (GremlinProperty property in items)
			{
				Add(property);
			}
		}

		protected override string GetKeyForItem(GremlinProperty item)
		{
			return item.Key;
		}
	}
}
