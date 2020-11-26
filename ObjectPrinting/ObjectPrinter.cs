using System;
using System.Collections.Generic;

namespace ObjectPrinting
{
    public class ObjectPrinter
    {
        public static PrintingConfig<T> For<T>()
        {
            return new PrintingConfig<T>();
        }
    }
}