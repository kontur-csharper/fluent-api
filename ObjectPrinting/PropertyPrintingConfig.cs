using System;
using System.Globalization;

namespace ObjectPrinting
{
    public class PropertyPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
    {
        private readonly Configurator<TOwner> configurator;

        Configurator<TOwner> IPropertyPrintingConfig<TOwner, TPropType>.ParentConfig => configurator;

        public PropertyPrintingConfig(Configurator<TOwner> configurator)
        {
            this.configurator = configurator;
        }

        public Configurator<TOwner> Using(Func<TPropType, string> print)
        {
            return configurator.AddPrintingMethod(print);
        }

        public Configurator<TOwner> Using(CultureInfo culture)
        {
            return configurator.AddPrintingCulture<TPropType>(culture);
        }
    }
}