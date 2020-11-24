using System;

namespace ObjectPrinting
{
    internal class ValueInfo
    {
        internal object Value { get; }
        internal Type Type { get; }
        internal string Name { get; }

        internal ValueInfo(object value, Type type, string name)
        {
            Value = value;
            Type = type;
            Name = name;
        }
    }
}