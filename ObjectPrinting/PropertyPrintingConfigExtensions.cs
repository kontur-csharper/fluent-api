using System;
using System.Collections.Generic;

namespace ObjectPrinting
{
    public static class PropertyPrintingConfigExtensions
    {
        public static string PrintToString<T>(this T obj, Func<PrintingConfig<T>, PrintingConfig<T>> config)
        {
            return config(ObjectPrinter.For<T>()).PrintToString(obj);
        }

        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(this PropertyPrintingConfig<TOwner, string> propConfig, int maxLen)
        {
            Func<string, string> trimFunc = s => s.Substring(0, Math.Min(s.Length, maxLen));
            if(!((IPropertyPrintingConfig<TOwner, string>)propConfig).ParentConfig.configuration.TrimmingProperties.ContainsKey(typeof(TOwner)))
                ((IPropertyPrintingConfig<TOwner, string>)propConfig).ParentConfig.configuration.TrimmingProperties[typeof(TOwner)]= new Dictionary<string, Delegate>();

            ((IPropertyPrintingConfig<TOwner, string>) propConfig)
                .ParentConfig.configuration.TrimmingProperties[typeof(TOwner)][propConfig.propertyName] = trimFunc;
            
            return ((IPropertyPrintingConfig<TOwner, string>)propConfig).ParentConfig;
        }
    }
}