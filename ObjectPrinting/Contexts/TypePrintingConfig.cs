using System;
using ObjectPrinting.Contracts;

namespace ObjectPrinting.Contexts
{
    public class TypePrintingConfig<TOwner, TType> : IContextPrintingConfig<TOwner, TType>
    {
        private IPrintingConfig PrintingConfig { get; }
        public TypePrintingConfig(IPrintingConfig printingConfig) => PrintingConfig = printingConfig;

        public PrintingConfig<TOwner> Using(Func<TType, string> print)
        {
            var newConfig = PrintingConfig.AddAlternativePrintingFor(typeof(TType), obj => print((TType) obj));
            return newConfig as PrintingConfig<TOwner>;
        }
    }
}