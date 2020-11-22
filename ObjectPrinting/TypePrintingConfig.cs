using System;

namespace ObjectPrinting
{
    public class TypePrintingConfig<TOwner, TPropType> : IConfig<TOwner, TPropType>
    {
        private readonly IPrintingConfig<TOwner> printingConfig;

        IPrintingConfig<TOwner> IConfig<TOwner, TPropType>.ParentConfig => printingConfig;

        public TypePrintingConfig(PrintingConfig<TOwner> printingConfig)
        {
            this.printingConfig = printingConfig;
        }

        public IPrintingConfig<TOwner> Using(Func<TPropType, string> print)
        {
            printingConfig.TypeSerialization[typeof(TPropType)] = print;
            return printingConfig;
        }
    }
}
