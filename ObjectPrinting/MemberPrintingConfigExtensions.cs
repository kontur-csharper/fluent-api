using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class MemberPrintingConfigExtensions
    {
        public static string PrintToString<T>(this T obj, Func<PrintingConfig<T>, PrintingConfig<T>> config)
        {
            return config(ObjectPrinter.For<T>()).PrintToString(obj);
        }

        public static PrintingConfig<TOwner> UsingCulture<TOwner, T>(
            this MemberPrintingConfig<TOwner, T> propConfig,
            CultureInfo culture)
            where T : IFormattable
        {
            return propConfig.Using(x => x.ToString(null, culture));
        }

        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(
            this MemberPrintingConfig<TOwner, string> propConfig, int maxLength)
        {
            if (maxLength < 0)
                throw new ArgumentException("maxLength must be non-negative");
            return propConfig.Using(s => maxLength > s.Length ? s : s.Substring(0, maxLength));
        }
    }
}