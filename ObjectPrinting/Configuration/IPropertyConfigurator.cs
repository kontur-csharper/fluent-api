using ObjectPrinting.Serializers;

namespace ObjectPrinting.Configuration
{
    public interface IPropertyConfigurator
    {
        IPropertySerializer AppliedSerializer { get; }
    }

    public interface IPropertyConfigurator<TOwner, TProperty> : IPropertyConfigurator
    {
        IPropertySerializer IPropertyConfigurator.AppliedSerializer => AppliedSerializer;

        new PropertySerializer<TProperty> AppliedSerializer { get; }
        PrintingConfigBuilder<TOwner> Using(PropertySerializer<TProperty> serializer);
    }
}