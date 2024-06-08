using System.Collections.Generic;
using System.Linq;

namespace tesinormed.FAndCDaveCo.Extensions;

public static class DictionaryExtensions
{
	public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
		=> dictionary.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);
}
