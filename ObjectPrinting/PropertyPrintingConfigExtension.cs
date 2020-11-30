using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class PropertyPrintingConfigExtension
    {
        public static string PrintToString<T>(this T obj, Func<PrintingConfig<T>, PrintingConfig<T>> config)
        {
            return config(ObjectPrinter.For<T>()).PrintToString(obj);
        }
        
        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(this PropertyPrintingConfig<TOwner, string> propConfig, int maxLen)
        {
            Func<string, string> trim = 
                str => (maxLen > str.Length) ? str : str.Substring(0, maxLen);
            var iPropConfig = (IPropertyPrintingConfig<TOwner, string>) propConfig;
            iPropConfig.ParentConfig.AddSerializationForProperty(iPropConfig.MemberName, trim, iPropConfig.DeclaringType);
            return iPropConfig.ParentConfig;
        }
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, int> propConfig, CultureInfo culture)
        {
            var propertyConfig = (IPropertyPrintingConfig<TOwner, int>) propConfig;
            propertyConfig.ParentConfig.AddCultureForMember(typeof(int), culture);
            return propertyConfig.ParentConfig;
        }
        
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, double> propConfig, CultureInfo culture)
        {
            var propertyConfig = (IPropertyPrintingConfig<TOwner, double>) propConfig;
            propertyConfig.ParentConfig.AddCultureForMember(typeof(double), culture);
            return propertyConfig.ParentConfig;
        }
        
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, float> propConfig, CultureInfo culture)
        {
            var propertyConfig = (IPropertyPrintingConfig<TOwner, float>) propConfig;
            propertyConfig.ParentConfig.AddCultureForMember(typeof(float), culture);
            return propertyConfig.ParentConfig;
        }
        
        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, DateTime> propConfig, CultureInfo culture)
        {
            var propertyConfig = (IPropertyPrintingConfig<TOwner, DateTime>) propConfig;
            propertyConfig.ParentConfig.AddCultureForMember(typeof(DateTime), culture);
            return propertyConfig.ParentConfig;
        }
    }
}