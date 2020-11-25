using System;

namespace ObjectPrinting.Serializers
{
    public interface IPropertySerializer
    {
        Type PropertyType { get; }
        string Serialize(object value);
    }
}