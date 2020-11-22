using System;

namespace ObjectPrinting
{
    public static class ObjectExtentions
    {
        public static string PrintToString<T>(this T obj, Func<IPrintingConfig<T>, IPrintingConfig<T>> config)
        {
            return config(ObjectPrinter.For<T>()).PrintToString(obj);
        }

        public static string PrintToString<T>(this T obj)
        {
            return ObjectPrinter.For<T>().PrintToString(obj);
        }
    }
}
