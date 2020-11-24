using System;

namespace ObjectPrinting
{
    public class PropertyPrintingConfig<TProperty, TOwner> : IPropertyPrintingConfig<TProperty, TOwner>
    {
        public PropertyPrintingConfig(IPrintingConfig<TOwner> config, string propertyName)
        {
            Config = config;
            Property = propertyName;
        }

        private IPrintingConfig<TOwner> Config { get; }
        private string Property { get; }

        public IPrintingConfig<TOwner> Using(Func<TProperty, string> func)
        {
            Config.AddSerialization(Property, func);

            return Config;
        }
    }
}