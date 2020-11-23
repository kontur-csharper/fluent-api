using System;
using System.Reflection;

namespace ObjectPrinting.Config
{
    public class FieldPrintingConfig<TOwner, TPropType> : IFieldPrintingConfig<TOwner, TPropType>
    {
        private readonly FieldInfo fieldInfo;
        private readonly IPrintingConfig<TOwner> printingConfig;

        public FieldPrintingConfig(IPrintingConfig<TOwner> printingConfig, FieldInfo fieldInfo)
        {
            this.printingConfig = printingConfig;
            this.fieldInfo = fieldInfo;
        }

        public IPrintingConfig<TOwner> Using(Func<TPropType, string> serializer)
        {
            printingConfig.FieldToSerializer[fieldInfo] = serializer;
            return printingConfig;
        }

        FieldInfo IFieldPrintingConfig<TOwner, TPropType>.FieldInfo => fieldInfo;
        IPrintingConfig<TOwner> IConfig<TOwner, TPropType>.ParentConfig => printingConfig;
    }
}