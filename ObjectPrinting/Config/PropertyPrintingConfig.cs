using System;
using System.Reflection;

namespace ObjectPrinting.Config
{
    public class PropertyPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
    {
        private readonly IPrintingConfig<TOwner> printingConfig;
        private readonly PropertyInfo propertyInfo;

        public PropertyPrintingConfig(IPrintingConfig<TOwner> printingConfig,
            PropertyInfo propertyInfo = null)
        {
            this.printingConfig = printingConfig;
            this.propertyInfo = propertyInfo;
        }

        public IPrintingConfig<TOwner> Using(Func<TPropType, string> serializer)
        {
            printingConfig.PropertyToSerializer[propertyInfo] = serializer;
            return printingConfig;
        }

        PropertyInfo IPropertyPrintingConfig<TOwner, TPropType>.PropertyInfo => propertyInfo;
        IPrintingConfig<TOwner> IConfig<TOwner, TPropType>.ParentConfig => printingConfig;
    }
}