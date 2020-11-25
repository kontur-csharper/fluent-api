using System;

namespace ObjectPrinting
{
    public class TypePrintingConfig<TOwner, T> : IMemberPrintingConfig<TOwner, T>
    {
        private readonly PrintingConfig<TOwner> printingConfig;
        public readonly Type SelectedType;

        public TypePrintingConfig(PrintingConfig<TOwner> printingConfig, Type selectedType)
        {
            this.printingConfig = printingConfig;
            SelectedType = selectedType;
        }

        public PrintingConfig<TOwner> Using(Func<T, string> print)
        {
            printingConfig.AddOwnSerializationForSelectedType(print, SelectedType);
            return printingConfig;
        }
    }
}