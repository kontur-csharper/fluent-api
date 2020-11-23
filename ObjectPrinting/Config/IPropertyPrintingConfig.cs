using System.Reflection;

namespace ObjectPrinting.Config
{
    public interface IPropertyPrintingConfig<TOwner, TPropType> : IConfig<TOwner, TPropType>
    {
        PropertyInfo PropertyInfo { get; }
    }
}
