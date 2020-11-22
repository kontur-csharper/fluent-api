using System;
using System.Reflection;

namespace ObjectPrinting
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

        public IPrintingConfig<TOwner> Using(Func<TPropType, string> print)
        {
            printingConfig.FieldSerialization[fieldInfo] = print;
            return printingConfig;
        }

        FieldInfo IFieldPrintingConfig<TOwner, TPropType>.FieldInfo => fieldInfo;
        IPrintingConfig<TOwner> IConfig<TOwner, TPropType>.ParentConfig => printingConfig;
    }
}
