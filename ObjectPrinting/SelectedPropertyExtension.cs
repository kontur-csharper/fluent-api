namespace ObjectPrinting
{
    internal static class SelectedPropertyExtension
    {
        public static PrintingConfig<TOwner> Trim<TOwner>(
            this SelectedProperty<TOwner, string> printingConfig, int length)
        {
            return printingConfig.UseSerializer(s => s.Substring(0, length));
        }
    }
}