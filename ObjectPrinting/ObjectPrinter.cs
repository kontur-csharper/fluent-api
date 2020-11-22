namespace ObjectPrinting
{
    public class ObjectPrinter
    {
        public static IPrintingConfig<T> For<T>()
        {
            return new PrintingConfig<T>();
        }
    }
}