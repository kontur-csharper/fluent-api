using ObjectPrinting.Core;

namespace ObjectPrinting.Interfaces
{
    internal interface IMemberPrintingConfig<TOwner>
    {
        PrintingConfig<TOwner> ParentConfig { get; }
    }
}