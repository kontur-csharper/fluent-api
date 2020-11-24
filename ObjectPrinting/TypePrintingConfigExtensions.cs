using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class TypePrintingConfigExtensions
    {
        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(this TypePrintingConfig<TOwner, string> propConfig,
            int maxLength)
        {
            return propConfig.Using(str => str.Substring(0, Math.Min(str.Length, maxLength)));
        }

        public static PrintingConfig<TOwner> Using<TOwner, TPropType>(
            this TypePrintingConfig<TOwner, TPropType> propConfig,
            CultureInfo culture) where TPropType : IFormattable
        {
            return propConfig.Using(obj => obj.ToString(null, culture));
        }
    }
}