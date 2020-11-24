using System;

namespace ObjectPrinting
{
    public interface IPropertyPrintingConfig<out TProperty, TOwner>
    {
        public IPrintingConfig<TOwner> Using(Func<TProperty, string> func);
    }
}