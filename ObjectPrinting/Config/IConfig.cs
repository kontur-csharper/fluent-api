using System;

namespace ObjectPrinting
{
    public interface IConfig<TOwner, TPropType>
    {
        IPrintingConfig<TOwner> ParentConfig { get; }
        public IPrintingConfig<TOwner> Using(Func<TPropType, string> print);
    }
}
