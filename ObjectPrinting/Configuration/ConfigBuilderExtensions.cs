namespace ObjectPrinting.Configuration
{
    public static class ConfigBuilderExtensions
    {
        public static string PrintToString<TOwner>(this PrintingConfigBuilder<TOwner> builder, TOwner obj) =>
            builder.Build().PrintToString(obj);
    }
}