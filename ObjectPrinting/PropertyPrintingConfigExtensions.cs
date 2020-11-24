namespace ObjectPrinting
{
    public static class PropertyPrintingConfigExtensions
    {
        public static IPrintingConfig<TOwner> Trimmed<TOwner>(this IPropertyPrintingConfig<string, TOwner> config,
            int lenght)
        {
            return config.Using(x => x[..^lenght]);
        }
    }
}