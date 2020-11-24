using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class PropertyPrintingConfigExtensions
    {
        public static PrintingConfig<TOwner> Using<TOwner, TPropType>(
            this PropertyPrintingConfig<TOwner, TPropType> printingConfig, CultureInfo cultureInfo)
            where TPropType : IFormattable
        {
            return printingConfig.Using(numericValue => string.Format(cultureInfo, "{0}", numericValue));
        }

        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(
            this PropertyPrintingConfig<TOwner, string> propConfig, int maxLen)
        {
            return maxLen < 0
                ? propConfig.Using(s => "")
                : propConfig.Using(s => s.Substring(0, Math.Min(s.Length, maxLen)));
        }
    }
}