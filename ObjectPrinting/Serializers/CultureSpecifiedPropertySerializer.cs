using System;
using System.Globalization;

namespace ObjectPrinting.Serializers
{
    public class CultureSpecifiedPropertySerializer<TProperty> : PropertySerializer<TProperty>
        where TProperty : IFormattable
    {
        private readonly string? format;
        private readonly CultureInfo cultureInfo;

        public CultureSpecifiedPropertySerializer(CultureInfo cultureInfo, string format)
        {
            this.cultureInfo = cultureInfo;
            this.format = format;
        }

        public CultureSpecifiedPropertySerializer(CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo;
            format = null;
        }

        public override string Serialize(TProperty value)
        {
            return value.ToString(format, cultureInfo);
        }
    }
}