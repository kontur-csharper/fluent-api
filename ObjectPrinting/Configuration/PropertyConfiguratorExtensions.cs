using System;
using System.Globalization;
using ObjectPrinting.Serializers;

namespace ObjectPrinting.Configuration
{
    public static class PropertyConfiguratorExtensions
    {
        public static PrintingConfigBuilder<TOwner> Exclude<TOwner, TProperty>(
            this IPropertyConfigurator<TOwner, TProperty> property)
        {
            return property.Using(new IgnorePropertySerializer<TProperty>());
        }

        public static PrintingConfigBuilder<TOwner> UseSerializer<TOwner, TProperty>(
            this IPropertyConfigurator<TOwner, TProperty> property,
            Func<TProperty, string> serializer)
        {
            return property.Using(new DelegatePropertySerializer<TProperty>(serializer));
        }

        public static PrintingConfigBuilder<TOwner> Trim<TOwner>(this IPropertyConfigurator<TOwner, string> property,
            int maxLength)
        {
            return property.Using(new TrimmedPropertySerializer(maxLength));
        }

        public static PrintingConfigBuilder<TOwner> SetCulture<TOwner, TProperty>(
            this IPropertyConfigurator<TOwner, TProperty> property, string format, CultureInfo cultureInfo)
            where TProperty : IFormattable
        {
            return property.Using(new CultureSpecifiedPropertySerializer<TProperty>(cultureInfo, format));
        }

        public static PrintingConfigBuilder<TOwner> SetCulture<TOwner, TProperty>(
            this IPropertyConfigurator<TOwner, TProperty> property, CultureInfo cultureInfo)
            where TProperty : IFormattable
        {
            return property.Using(new CultureSpecifiedPropertySerializer<TProperty>(cultureInfo, null));
        }
    }
}