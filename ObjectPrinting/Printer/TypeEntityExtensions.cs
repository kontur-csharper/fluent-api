using System;
using System.Globalization;

namespace ObjectPrinting.Printer
{
    public static class TypeEntityExtensions
    {
        public static PrintingConfig<TOwner> SetCulture<TOwner, T>(this TypeEntity<TOwner, T> config,
            CultureInfo culture)
            where T : IFormattable
        {
            config.Parent.Cultures.Add(typeof(T), culture);
            return config.Parent;
        }

        public static PrintingConfig<TOwner> CropTo<TOwner>(this TypeEntity<TOwner, string> config, int maxLength)
        {
            config.Parent.MaximumStringLength = maxLength;
            return config.Parent;
        }
    }
}
