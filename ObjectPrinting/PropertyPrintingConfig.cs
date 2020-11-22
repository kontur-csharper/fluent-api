using System;
using System.Reflection;

namespace ObjectPrinting
{
    public class PropertyPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
    {
        private readonly IPrintingConfig<TOwner> printingConfig;
        private readonly PropertyInfo propertyInfo;


        public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig,
            PropertyInfo propertyInfo = null)
        {
            this.printingConfig = printingConfig;
            this.propertyInfo = propertyInfo;
        }

        public IPrintingConfig<TOwner> Using(Func<TPropType, string> print)
        {
            printingConfig.PropertySerialization[propertyInfo] = print;
            return printingConfig;
        }

        IPrintingConfig<TOwner> IConfig<TOwner, TPropType>.ParentConfig => printingConfig;
        PropertyInfo IPropertyPrintingConfig<TOwner, TPropType>.PropertyInfo => propertyInfo;
    }
}
