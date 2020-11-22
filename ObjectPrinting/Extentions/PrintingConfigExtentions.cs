using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class PrintingConfigExtentions
    {
        public static IPrintingConfig<TOwner> Using<TOwner, TPropType>(
            this IConfig<TOwner, TPropType> config,
            CultureInfo cultureInfo) where TPropType : IFormattable
        {
            Func<TPropType, string> serializer = obj => obj.ToString(null, cultureInfo);
            return config.Using(serializer);
        }

        public static IPrintingConfig<TOwner> TrimmedToLength<TOwner>(
            this IConfig<TOwner, string> config, int maxLen)
        {
            Func<string, string> serializer = str => str.Substring(0, Math.Min(str.Length, maxLen));
            return config.Using(serializer);

        }
    }
}
