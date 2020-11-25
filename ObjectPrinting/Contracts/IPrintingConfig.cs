using System;
using System.Reflection;

namespace ObjectPrinting.Contracts
{
    public interface IPrintingConfig
    {
        AlternativePrinter AlternativePrinter { get; }
        PrintExcluder PrintExcluder { get; }

        IPrintingConfig AddAlternativePrintingFor(Type type, Func<object, string> print);

        IPrintingConfig AddAlternativePrintingFor(PropertyInfo property, Func<object, string> print);

        IPrintingConfig AddAlternativePrintingFor(FieldInfo field, Func<object, string> print);
    }
}