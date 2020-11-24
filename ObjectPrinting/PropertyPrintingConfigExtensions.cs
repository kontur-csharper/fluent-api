using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class PropertyPrintingConfigExtensions
    {
        public static string PrintToString<T>(this T obj, Func<PrintingConfig<T>, PrintingConfig<T>> config)
        {
            return config(ObjectPrinter.For<T>()).PrintToString(obj);
        }

        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(
            this PropertyPrintingConfig<TOwner, string> propConfig,
            int maxLen)
        {
            var parentConfig = ((IPropertyPrintingConfig<TOwner, string>) propConfig).ParentConfig;
            string Trim(string s)
            {
                if (s == null) return null;
                var length = Math.Max(0, Math.Min(s.Length, maxLen));
                return s.Substring(0, length);
            }

            parentConfig.AddRule( (Func<string, string>) Trim);
            return parentConfig;
        }

        public static PrintingConfig<TOwner> Using<TOwner, TPropType>(
            this PropertyPrintingConfig<TOwner, TPropType> propConfig, 
            CultureInfo culture) 
            where TPropType : IFormattable
        {
            var parentConfig = ((IPropertyPrintingConfig<TOwner, TPropType>) propConfig).ParentConfig;
            parentConfig.AddCulture<TPropType>(culture);
            return parentConfig;
        }
    }
}