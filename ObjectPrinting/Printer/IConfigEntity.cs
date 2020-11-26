using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectPrinting.Printer
{
    public interface IConfigEntity<TOwner>
    {
        PrintingConfig<TOwner> Parent { get; }
    }
}
