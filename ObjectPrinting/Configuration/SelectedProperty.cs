using ObjectPrinting.Serializers;

namespace ObjectPrinting.Configuration
{
    /// <summary>
    /// Single selected property to be serialized
    /// </summary>
    public class SelectedProperty<TOwner, TProperty> : IPropertyConfigurator<TOwner, TProperty>
    {
        public SelectedProperty(SerializationTarget target, PrintingConfigBuilder<TOwner> parent)
        {
            Target = target;
            Owner = parent;
        }

        public PropertySerializer<TProperty>? AppliedSerializer { get; private set; }
        public SerializationTarget Target { get; }
        public PrintingConfigBuilder<TOwner> Owner { get; }

        public PrintingConfigBuilder<TOwner> Using(PropertySerializer<TProperty> serializer)
        {
            AppliedSerializer = serializer;
            return Owner;
        }
    }
}