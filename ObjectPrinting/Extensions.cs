using System;
using System.Collections.Immutable;
using System.Globalization;

namespace ObjectPrinting
{
    public static class ImmutableDictionaryExtensions
    {
        public static ImmutableDictionary<TKey, TValue> AddOrUpdate<TKey, TValue>(
            this ImmutableDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
                return dict.SetItem(key, value);
            return dict.Add(key, value);
        }
    }

    public static class PropertyPrintingConfigExtensions
    {
        public static PrintingConfig<TOwner> SetCulture<TOwner, TProperty>(
            this MemberPrintingConfig<TOwner, TProperty> propConfig, CultureInfo culture)
            where TProperty : IFormattable
        {
            return propConfig.SetSerializer(x => ((IFormattable)x).ToString("N", culture));
        }

        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(
            this MemberPrintingConfig<TOwner, string> propConfig, int maxLength)
        {
            return propConfig.SetSerializer(str => str.Substring(0, Math.Min(str.Length, maxLength)));
        }
    }
    public static class ObjectExtensions
    {
        public static string PrintToString<T>(this T obj)
        {
            return ObjectPrinter<T>.Should().Build().PrintToString(obj);
        }
    }
}
