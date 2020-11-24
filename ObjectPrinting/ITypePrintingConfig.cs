using System;

namespace ObjectPrinting
{
    public interface ITypePrintingConfig<TOwner, out TPropertyType>
    {
        public IPrintingConfig<TOwner> Using(Func<TPropertyType, string> func);
    }
}