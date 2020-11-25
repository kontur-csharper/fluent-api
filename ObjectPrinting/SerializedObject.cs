namespace ObjectPrinting
{
    public class SerializedObject
    {
        public readonly int NestingLevel;
        public readonly object Object;

        public SerializedObject(object obj, int nestingLevel)
        {
            Object = obj;
            NestingLevel = nestingLevel;
        }
    }
}