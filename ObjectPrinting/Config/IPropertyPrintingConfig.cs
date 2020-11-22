using System.Reflection;

namespace ObjectPrinting
{
    public interface IPropertyPrintingConfig<TOwner, TPropType> : IConfig<TOwner, TPropType>
    {
        PropertyInfo PropertyInfo { get; }
    }
}
