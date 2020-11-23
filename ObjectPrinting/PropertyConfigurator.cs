using System;
using System.Globalization;

namespace ObjectPrinting
{
    public class PropertyConfigurator<TOwner, TPropType> : IPropertyConfigurator<TOwner, TPropType>
    {
        private readonly Configurator<TOwner> printingConfig;
        private readonly Action<Func<TPropType, string>> printingFunction;

        public PropertyConfigurator(Configurator<TOwner> printingConfig, Action<Func<TPropType, string>> func)
        {
            this.printingConfig = printingConfig;
            printingFunction = func;
        }

        public Configurator<TOwner> Using(Func<TPropType, string> print)
        {
            printingFunction(print);
            return printingConfig;
        }

        public Configurator<TOwner> Using(CultureInfo culture)
        {
            return printingConfig.AddCultureForType<TPropType>(culture);
        }
    }
}