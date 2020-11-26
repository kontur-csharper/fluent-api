using System;

namespace ObjectPrinting.Infrastructure
{
    public static class PropertyPrintingConfigExtensions
    {
        public static string PrintToString<T>(this T obj, Func<PrintingConfig<T>, PrintingConfig<T>> config)
        {
            return config(ObjectPrinter.For<T>()).PrintToString(obj);
        }

        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(
            this PropertyPrintingConfig<TOwner, string> propConfig, 
            int maxLength)
        {
            var printer= ((IPropertyPrintingConfig<TOwner, string>)propConfig).ParentConfig;
            propConfig.SetLengthConstraint(maxLength);
            return printer;
        }

    }
}