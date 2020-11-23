using System;
using System.Globalization;

namespace ObjectPrinting
{
    public interface IPropertyConfigurator<TOwner, out TPropType>
    {
        Configurator<TOwner> Using(Func<TPropType, string> print);
        Configurator<TOwner> Using(CultureInfo culture);
    }
}