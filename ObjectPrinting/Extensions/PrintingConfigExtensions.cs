using ObjectPrinting.Config;
using System;
using System.Globalization;

namespace ObjectPrinting.Extensions
{
    public static class PrintingConfigExtensions
    {
        public static IPrintingConfig<TOwner> Using<TOwner, TPropType>(
            this IConfig<TOwner, TPropType> config,
            CultureInfo cultureInfo) where TPropType : IFormattable
        {
            return config.Using(obj => obj.ToString(null, cultureInfo));
        }

        public static IPrintingConfig<TOwner> TrimmedToLength<TOwner>(
            this IConfig<TOwner, string> config, int maxLen)
        {
            return config.Using(str => str.Substring(0, Math.Min(str.Length, maxLen)));
        }
    }
}
