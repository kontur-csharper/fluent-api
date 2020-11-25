using System;

namespace ObjectPrinting
{
    public static class ObjectPrinter
    {
        public static PrintingConfigBuilder<T> For<T>() => PrintingConfigBuilder<T>.Default();

        public static string Serialize<T>(this T target) =>
            PrintingConfigBuilder<T>.Default().Build().PrintToString(target);

        public static string Serialize<T>(this T target, Action<PrintingConfigBuilder<T>> configurator)
        {
            var printingConfig = For<T>();
            configurator.Invoke(printingConfig);
            return printingConfig.Build().PrintToString(target);
        }
    }
}