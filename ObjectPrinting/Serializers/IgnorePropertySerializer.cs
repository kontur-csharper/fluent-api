namespace ObjectPrinting.Serializers
{
    public class IgnorePropertySerializer<TProperty> : PropertySerializer<TProperty>
    {
        public override string Serialize(TProperty value) => string.Empty;
    }
}