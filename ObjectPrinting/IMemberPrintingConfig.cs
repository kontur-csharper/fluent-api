using System;

namespace ObjectPrinting
{
    public interface IMemberPrintingConfig<TOwner, TPropType>
    {
        public PrintingConfig<TOwner> Using(Func<TPropType, string> print);
    }
}