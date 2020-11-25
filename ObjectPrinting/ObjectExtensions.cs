namespace ObjectPrinting
{
    public static class ObjectExtensions
    {
        public static string PrintToString<T>(this T obj)
        {
            return Printer.PrintToString(obj, x => x);
        }
    }
}