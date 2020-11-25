using System;

namespace ObjectPrinting.Extensions
{
    public static class PrintingConfigExtensions
    {
        public static string PrintToString<T>(this T entity) =>
            ObjectPrinter.For<T>().PrintToString(entity);

        public static string PrintToString<T>(this T entity, Func<PrintingConfig<T>, PrintingConfig<T>> config) =>
            config(ObjectPrinter.For<T>()).PrintToString(entity);
    }
}