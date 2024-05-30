using System.Collections.Generic;
using System.Linq;

namespace tesinormed.FAndCDaveCo
{
	public static class DictionaryExtensions
	{
		public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> dictionary) => dictionary
			.ToDictionary(keySelector: (kv) => kv.Key, elementSelector: (kv) => kv.Value);
	}
}
