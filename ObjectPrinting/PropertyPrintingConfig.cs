using System;
using System.Linq.Expressions;

namespace ObjectPrinting
{
    public class PropertyPrintingConfig<TOwner, TPropType> : IPropertyPrintingConfig<TOwner, TPropType>
    {
        private PrintingConfig<TOwner> PrintingConfig { get; }
        private readonly Expression<Func<TOwner, TPropType>> memberSelector;

        internal PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig,
            Expression<Func<TOwner, TPropType>> memberSelector = null)
        {
            PrintingConfig = printingConfig;
            this.memberSelector = memberSelector;
        }

        public PrintingConfig<TOwner> Using(Func<TPropType, string> print)
        {
            if (memberSelector is null)
                return new PrintingConfig<TOwner>(PrintingConfig.ConfigurationInfo.AddUsingForType(print));
            return new PrintingConfig<TOwner>(
                PrintingConfig.ConfigurationInfo.AddUsingForProperty(print, memberSelector.GetObjectName()));
        }

        PrintingConfig<TOwner> IPropertyPrintingConfig<TOwner, TPropType>.ParentConfig => PrintingConfig;
    }
}