using System;
using System.Reflection;
using ObjectPrinting.Contracts;

namespace ObjectPrinting.Contexts
{
    public class PropertyPrintingConfig<TOwner, TPropType> : IContextPrintingConfig<TOwner, TPropType>
    {
        private PropertyInfo Property { get; }
        private IPrintingConfig PrintingConfig { get; }

        public PropertyPrintingConfig(IPrintingConfig printingConfig, PropertyInfo property)
        {
            Property = property;
            PrintingConfig = printingConfig;
        }

        public PrintingConfig<TOwner> Using(Func<TPropType, string> print)
        {
            var newConfig = PrintingConfig.AddAlternativePrintingFor(Property, obj => print((TPropType) obj));
            return newConfig as PrintingConfig<TOwner>;
        }
    }
}