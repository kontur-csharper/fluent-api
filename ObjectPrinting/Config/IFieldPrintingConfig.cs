using System.Reflection;

namespace ObjectPrinting.Config
{
    public interface IFieldPrintingConfig<TOwner, TPropType> : IConfig<TOwner, TPropType>
    {
        FieldInfo FieldInfo { get; }
    }
}
