using System;

namespace ObjectPrinting.Config
{
    public interface IConfig<TOwner, TPropType>
    {
        IPrintingConfig<TOwner> ParentConfig { get; }
        public IPrintingConfig<TOwner> Using(Func<TPropType, string> serializer);
    }
}
