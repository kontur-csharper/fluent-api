namespace ObjectPrinting
{
    public static class PrintingConfigExtensions
    {
        public static string PrintToString<T>(this PrintingConfig<T> printingConfig, T objectToSerialize)
        {
            return new Serializer<T>(printingConfig).Serialize(objectToSerialize);
        }
    }
}