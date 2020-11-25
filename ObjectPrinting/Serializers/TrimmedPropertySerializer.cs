namespace ObjectPrinting.Serializers
{
    public class TrimmedPropertySerializer : PropertySerializer<string>
    {
        private readonly int maxLength;

        public TrimmedPropertySerializer(int maxLength)
        {
            this.maxLength = maxLength;
        }

        public override string Serialize(string value)
        {
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}