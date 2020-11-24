using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class MemberPrintingConfigExtensions
    {
        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(
            this MemberPrintingConfig<TOwner, string> memberConfig, int maxLen)
        {
            return memberConfig.Using(member => member.Length < maxLen ?
                member : 
                member.Substring(0, maxLen));
        }

        public static PrintingConfig<TOwner> Using<TOwner, TMemberType>(
            this MemberPrintingConfig<TOwner, TMemberType> memberConfig, CultureInfo culture)
            where TMemberType : IFormattable
        {
            return memberConfig.Using(member => member.ToString(null, culture));
        }
    }
}