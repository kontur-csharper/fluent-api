using System;
using System.Reflection;

namespace ObjectPrinting
{
    public class PropertyPrintingConfig<TOwner, TProperty>
    {
        private PrintingConfig<TOwner> printingConfig;
        private PropertyInfo propertyToCustomize;
        
        public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig)
        {
            this.printingConfig = printingConfig;
        }
        
        public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig, 
            PropertyInfo propertyToCustomize)
        {
            this.printingConfig = printingConfig;
            this.propertyToCustomize = propertyToCustomize;
        }

        public PrintingConfig<TOwner> Using(Func<TProperty, string> serialization)
        {
            return printingConfig.SetCustomSerialization(serialization, propertyToCustomize);
        }
    }
}