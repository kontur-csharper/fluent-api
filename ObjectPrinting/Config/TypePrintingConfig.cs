using System;

namespace ObjectPrinting.Config
{
    public class TypePrintingConfig<TOwner, TPropType> : IConfig<TOwner, TPropType>
    {
        private readonly IPrintingConfig<TOwner> printingConfig;

        public TypePrintingConfig(IPrintingConfig<TOwner> printingConfig)
        {
            this.printingConfig = printingConfig;
        }

        public IPrintingConfig<TOwner> Using(Func<TPropType, string> serializer)
        {
            printingConfig.TypeToSerializer[typeof(TPropType)] = serializer;
            return printingConfig;
        }

        IPrintingConfig<TOwner> IConfig<TOwner, TPropType>.ParentConfig => printingConfig;
    }
}
