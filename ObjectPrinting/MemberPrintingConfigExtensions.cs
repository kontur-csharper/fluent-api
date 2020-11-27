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

        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(this MemberPrintingConfig<TOwner, string> propConfig, int maxLen)
        {
            if (maxLen < 0)
                throw new ArgumentException("Max length can not be less than zero");
            var serializer = new Func<string, string>(x => x.Length > maxLen ? x.Substring(0, maxLen) : x);
            var config = ((IPropertyPrintingConfig<TOwner, string>)propConfig).ParentConfig;

            if (propConfig.Info is null)
                config.AlternativeSerializersForTypes[typeof(string)] = serializer;
            else
                config.AlternativeSerializersForMembers[propConfig.Info] = serializer;
                    
            return config;
        }

        public static PrintingConfig<TOwner> UsingCulture<TOwner, T>(this MemberPrintingConfig<TOwner, T> propConfig,
            CultureInfo info) where T : IFormattable
        {
            return propConfig.Using(x => x.ToString(null, info));
        }
    }
}