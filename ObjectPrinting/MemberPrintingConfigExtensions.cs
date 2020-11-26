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

        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(
            this MemberPrintingConfig<TOwner, string> propConfig, int maxLen)
        {
            return propConfig.Using(x => x is null ? "null" : x.Substring(0, Math.Min(x.Length, maxLen)));
        }

        public static PrintingConfig<TOwner> Using<TOwner, TPropType>(
            this MemberPrintingConfig<TOwner, TPropType> propConfig, CultureInfo culture) where TPropType : IFormattable
        {
            return propConfig.Using(x => x.ToString(null, culture));
        }
    }
}