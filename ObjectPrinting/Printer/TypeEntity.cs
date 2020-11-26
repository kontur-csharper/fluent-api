using System;
using System.Collections.Generic;

namespace ObjectPrinting.Printer
{
    public class TypeEntity<TOwner, T> : IConfigEntity<TOwner>
    {
        private readonly PrintingConfig<TOwner> config;
        private readonly Dictionary<Type, Delegate> serializeWays;

        public TypeEntity(PrintingConfig<TOwner> config, Dictionary<Type, Delegate> serializeWays)
        {
            this.config = config;
            this.serializeWays = serializeWays;
        }

        public PrintingConfig<TOwner> SetSerializeWay(Func<T, string> serializer)
        {
            serializeWays.Add(typeof(T), serializer);
            return config;
        }

        public PrintingConfig<TOwner> Parent => config;
    }
}
