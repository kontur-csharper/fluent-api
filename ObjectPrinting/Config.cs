using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectPrinting
{
    public class Config
    {
        internal readonly List<PropertyInfo> ExcludedFields = new List<PropertyInfo>();
        internal readonly List<Type> ExcludedTypes = new List<Type>();

        internal readonly Dictionary<PropertyInfo, Delegate>
            FieldSerializers = new Dictionary<PropertyInfo, Delegate>();

        internal readonly Dictionary<Type, Delegate> TypesSerializer = new Dictionary<Type, Delegate>();

        internal bool IsExcluded(PropertyInfo propertyInfo)
        {
            return ExcludedFields.Contains(propertyInfo) || ExcludedTypes.Contains(propertyInfo.PropertyType);
        }

        internal bool IsSpecialSerialize(PropertyInfo propertyInfo, object element, out string result)
        {
            result = "";
            if (!FieldSerializers.ContainsKey(propertyInfo))
                return false;
            result = FieldSerializers[propertyInfo].DynamicInvoke(element)?.ToString();
            return true;
        }

        internal bool IsSpecialSerialize(Type type, object element, out string result)
        {
            result = "";
            if (!TypesSerializer.ContainsKey(type))
                return false;
            result = TypesSerializer[type].DynamicInvoke(element)?.ToString();
            return true;
        }
    }
}