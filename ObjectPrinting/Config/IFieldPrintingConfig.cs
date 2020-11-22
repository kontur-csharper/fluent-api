using System.Reflection;

namespace ObjectPrinting
{
    public interface IFieldPrintingConfig<TOwner, TPropType> : IConfig<TOwner, TPropType>
    {
        FieldInfo FieldInfo { get; }
    }
}
