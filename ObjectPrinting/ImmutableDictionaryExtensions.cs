using System.Collections.Immutable;

namespace ObjectPrinting
{
    public static class ImmutableDictionaryExtensions
    {
        public static ImmutableDictionary<TKey, TValue> AddOrSet<TKey, TValue>(
            this ImmutableDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue value)
        {
            return dictionary.ContainsKey(key)
                ? dictionary.SetItem(key, value)
                : dictionary.Add(key, value);
        }
    }
}