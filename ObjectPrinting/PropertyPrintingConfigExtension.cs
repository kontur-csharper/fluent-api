using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class PropertyPrintingConfigExtension
    {
        public static PrintingConfig<TOwner> Using<TOwner, TProperty>(
            this PropertyPrintingConfig<TOwner, TProperty> printingConfig,
            CultureInfo culture) where TProperty : IFormattable
        {
            return printingConfig.Using(number => string.Format(culture, "{0}", number));
        }
        
        public static PrintingConfig<TOwner> Trimmed<TOwner>(
            this PropertyPrintingConfig<TOwner, string> printingConfig, int length)
        {
            return printingConfig.Using(str 
                => str.Substring(0, length));
        }
    }
}