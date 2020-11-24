using System.Collections.Immutable;

namespace ObjectPrinting
{
    public static class ImmutableDictionaryExtensions
    {
        public static ImmutableDictionary<TKey, TValue> AddOrReplace<TKey, TValue>(
            this ImmutableDictionary<TKey, TValue> dic,
            TKey key, TValue value)
        {
            ImmutableDictionary<TKey, TValue> result;
            if (dic.ContainsKey(key))
                result = dic.Remove(key);
            result = dic.Add(key, value);
            return result;
        }
    }
}