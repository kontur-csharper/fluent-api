using System;
using System.Reflection;
using ObjectPrinting.Contracts;

namespace ObjectPrinting.Contexts
{
    public class FieldPrintingConfig<TOwner, TField> : IContextPrintingConfig<TOwner, TField>
    {
        private IPrintingConfig PrintingConfig { get; }
        private FieldInfo Field { get; }

        public FieldPrintingConfig(IPrintingConfig printingConfig, FieldInfo field)
        {
            PrintingConfig = printingConfig;
            Field = field;
        }

        public PrintingConfig<TOwner> Using(Func<TField, string> print)
        {
            var newConfig = PrintingConfig.AddAlternativePrintingFor(Field, obj => print((TField) obj));
            return newConfig as PrintingConfig<TOwner>;
        }
    }
}