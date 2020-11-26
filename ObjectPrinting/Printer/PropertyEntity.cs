using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ObjectPrinting.Printer
{
    public class PropertyEntity<TOwner, T> : PrintingConfig<TOwner>, IConfigEntity<TOwner>
    {
        private readonly PrintingConfig<TOwner> config;
        private readonly Dictionary<MemberInfo, Delegate> serializeWays;
        private readonly MemberInfo property;

        public PropertyEntity(PrintingConfig<TOwner> config, MemberInfo property, Dictionary<MemberInfo, Delegate> serializeWays)
        {
            this.config = config;
            this.serializeWays = serializeWays;
            this.property = property;
        }

        public PropertyEntity<TOwner, T> SetSerializeWay(Func<T, string> serializer)
        {
            serializeWays.Add(property, serializer);
            return this;
        }

        public PrintingConfig<TOwner> Parent => config;
    }
}
