using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class PropertyPrintingConfigExtensions
    {
        public static PrintingConfig<TOwner> SetMaxLength<TOwner>(
            this PropertyPrintingConfig<TOwner, string> propConfig, int maxLen)
        {
            var propertyConfig = ((IPropertyPrintingConfig<TOwner, string>) propConfig).ParentConfig;

            propertyConfig.SetMaxLength(maxLen);
            return propertyConfig;
        }

        public static PrintingConfig<TOwner> Using<TOwner, TPropType>(
            this PropertyPrintingConfig<TOwner, TPropType> propConfig, CultureInfo culture)
            where TPropType : IFormattable
        {
            var printingConfig = ((IPropertyPrintingConfig<TOwner, TPropType>) propConfig).ParentConfig;

            printingConfig.SetCulture<TPropType>(culture);
            return printingConfig;
        }
    }
}