using System;
using System.Globalization;
using ObjectPrinting.Core;
using ObjectPrinting.Interfaces;

namespace ObjectPrinting.Extensions
{
    public static class MemberPrintingConfigExtensions
    {
        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(
            this MemberPrintingConfig<TOwner, string> memberConfig, int maxLength)
        {
            if (maxLength <= 0)
                throw new Exception("Parameter maxLength must be positive");
            memberConfig.Using(p => p.Length > maxLength ? p.Substring(0, maxLength) : p);
            return ((IMemberPrintingConfig<TOwner>) memberConfig).ParentConfig;
        }

        public static PrintingConfig<TOwner> SpecifyCulture<TOwner, T>(
            this MemberPrintingConfig<TOwner, T> memberConfig, CultureInfo culture) where T : IFormattable
        {
            memberConfig.Using(prop => prop.ToString(null, culture));
            return ((IMemberPrintingConfig<TOwner>) memberConfig).ParentConfig;
        }
    }
}