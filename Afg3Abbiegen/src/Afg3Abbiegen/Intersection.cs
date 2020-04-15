using System.Collections.Generic;

namespace Afg3Abbiegen
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrCreateValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
            where TValue : new()
        {
            if (dict.TryGetValue(key, out var result)) return result;

            return dict[key] = new TValue();
        }
    }
}
