namespace ObjectPrinting
{
    public static class PropertyConfiguratorExtensions
    {
        public static Configurator<TOwner> TrimmedToLength<TOwner>(
            this IPropertyConfigurator<TOwner, string> propConfig, int maxLen)
        {
            return propConfig.Using(x => x.Length > maxLen ? x.Substring(0, maxLen) : x);
        }
    }
}