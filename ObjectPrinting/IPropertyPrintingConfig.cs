using System;
using System.Globalization;

namespace ObjectPrinting
{
    public interface IPropertyPrintingConfig<TOwner, TPropType>
    {
        Configurator<TOwner> ParentConfig { get; }
        public Configurator<TOwner> Using(Func<TPropType, string> print);
        public Configurator<TOwner> Using(CultureInfo culture);
    }
}