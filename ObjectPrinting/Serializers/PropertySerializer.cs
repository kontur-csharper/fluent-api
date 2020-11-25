using System;

namespace ObjectPrinting.Serializers
{
    /// <summary>
    /// General property serializer
    /// </summary>
    public abstract class PropertySerializer<TProperty> : IPropertySerializer
    {
        public abstract string Serialize(TProperty value);

        Type IPropertySerializer.PropertyType { get; } = typeof(TProperty);

        string IPropertySerializer.Serialize(object value)
        {
            if (value is TProperty prop)
                return Serialize(prop);

            var serializerInfo = $"serializer type: [{GetType()}], prop type: [{typeof(TProperty).Name}]";
            throw new ArgumentException($"Attempted to serialize object of type [{value.GetType()}]; {serializerInfo}");
        }
    }
}