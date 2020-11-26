using System;

namespace ObjectPrinting
{
    public partial class PrintingConfig<TOwner>
    {
        public class TypePrintingContext<TMember> : PrintingContext<TMember>
        {
            private readonly Type type;

            public TypePrintingContext(PrintingConfig<TOwner> ownerConfig, Type type)
            {
                OwnerConfig = ownerConfig;

                if (!OwnerConfig.TypeConfigs.TryGetValue(type, out var currentConfig))
                    currentConfig = new PrintingConfig<TMember>();
                CurrentConfig = (PrintingConfig<TMember>) currentConfig;

                this.type = type;
            }

            protected override PrintingConfig<TOwner> Apply(PrintingConfig<TMember> config)
            {
                var result = (PrintingConfig<TOwner>) OwnerConfig.Copy();
                result.TypeConfigs = result.TypeConfigs.SetItem(type, config);
                return result;
            }
        }
    }
}