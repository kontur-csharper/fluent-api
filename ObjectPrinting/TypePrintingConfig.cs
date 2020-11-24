using System;

namespace ObjectPrinting
{
    public class TypePrintingConfig<TOwner, TPropertyType> : ITypePrintingConfig<TOwner, TPropertyType>
    {
        public TypePrintingConfig(IPrintingConfig<TOwner> config)
        {
            Config = config;
        }

        private IPrintingConfig<TOwner> Config { get; }

        public IPrintingConfig<TOwner> Using(Func<TPropertyType, string> func)
        {
            Config.AddSerialization(typeof(TPropertyType), func);

            return Config;
        }
    }
}