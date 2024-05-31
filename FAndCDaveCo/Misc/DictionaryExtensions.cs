using System.Collections.Generic;
using System.Linq;
using tesinormed.FAndCDaveCo.Misc;

namespace tesinormed.FAndCDaveCo.Misc
{
	public static class DictionaryExtensions
	{
		public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> dictionary) => dictionary
			.ToDictionary(keySelector: (kv) => kv.Key, elementSelector: (kv) => kv.Value);
	}
}
