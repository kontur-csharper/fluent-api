using System;

namespace ObjectPrinting
{
    public partial class PrintingConfig<TOwner>
    {
        public abstract class PrintingContext<TMember>
        {
            protected PrintingConfig<TOwner> OwnerConfig;
            protected PrintingConfig<TMember> CurrentConfig;

            public PrintingConfig<TOwner> Using(PrintingConfig<TMember> config) => Apply(config);

            public PrintingConfig<TOwner> Using(Func<PrintingConfig<TMember>, PrintingConfig<TMember>> configSelector)
                => Apply(configSelector(CurrentConfig));

            public PrintingConfig<TOwner> As(Func<TMember, string> func)
                => Apply(CurrentConfig.WithPrintFunction(func));

            protected abstract PrintingConfig<TOwner> Apply(PrintingConfig<TMember> config);
        }
    }
}