using ObjectPrinting.Serializers;

namespace ObjectPrinting.Configuration
{
    /// <summary>
    /// Group of fields or properties with same type to be serialized
    /// </summary>
    public class SelectedPropertyGroup<TOwner, TProperty> : IPropertyConfigurator<TOwner, TProperty>
    {
        public SelectedPropertyGroup(PrintingConfigBuilder<TOwner> parent)
        {
            Owner = parent;
        }

        public PropertySerializer<TProperty>? AppliedSerializer { get; private set; }
        public PrintingConfigBuilder<TOwner> Owner { get; }

        public PrintingConfigBuilder<TOwner> Using(PropertySerializer<TProperty> serializer)
        {
            AppliedSerializer = serializer;
            return Owner;
        }
    }
}