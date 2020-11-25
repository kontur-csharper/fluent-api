using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class IMemberPrintingConfigExtensions
    {
        public static string PrintToString<T>(this T obj, Func<PrintingConfig<T>, PrintingConfig<T>> config)
        {
            return config(ObjectPrinter.For<T>()).PrintToString(obj);
        }

        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(
            this IMemberPrintingConfig<TOwner, string> propConfig, int maxLen)
        {
            return propConfig.Using(str => str.Length > maxLen ? str.Substring(0, maxLen) : str);
        }

        public static PrintingConfig<TOwner> Using<TOwner>(this IMemberPrintingConfig<TOwner, int> propConfig,
            CultureInfo culture)
        {
            return propConfig.Using(obj => obj.ToString(culture));
        }

        public static PrintingConfig<TOwner> Using<TOwner>(this IMemberPrintingConfig<TOwner, double> propConfig,
            CultureInfo culture)
        {
            return propConfig.Using(obj => obj.ToString(culture));
        }
    }
}