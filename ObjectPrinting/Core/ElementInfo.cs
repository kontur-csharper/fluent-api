using System;
using System.Reflection;

namespace ObjectPrinting.Core
{
    public class ElementInfo
    {
        public readonly Type ElementType;
        public readonly string FullName;
        public readonly string Name;
        private readonly Func<object, object> _getValueFunc;

        public object GetValue(object instance) => _getValueFunc(instance);

        public ElementInfo(PropertyInfo propertyInfo, string fullName)
        {
            ElementType = propertyInfo.PropertyType;
            FullName = $"{fullName}.{propertyInfo.Name}";
            Name = propertyInfo.Name;
            _getValueFunc = propertyInfo.GetValue;
        }

        public ElementInfo(FieldInfo fieldInfo, string fullName)
        {
            ElementType = fieldInfo.FieldType;
            FullName = $"{fullName}.{fieldInfo.Name}";
            Name = fieldInfo.Name;
            _getValueFunc = fieldInfo.GetValue;
        }
    }
}