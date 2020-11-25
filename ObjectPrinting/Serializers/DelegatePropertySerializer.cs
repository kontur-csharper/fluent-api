using System;

namespace ObjectPrinting.Serializers
{
    public class DelegatePropertySerializer<TProperty> : PropertySerializer<TProperty>
    {
        private readonly Func<TProperty, string> serializer;

        public DelegatePropertySerializer(Func<TProperty, string> serializer)
        {
            this.serializer = serializer;
        }

        public override string Serialize(TProperty value) => serializer.Invoke(value);
    }
}