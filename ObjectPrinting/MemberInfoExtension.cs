using System;
using System.Reflection;

namespace ObjectPrinting
{
    public static class MemberInfoExtension
    {
        public static Type Type(this MemberInfo info)
        {
            return info switch
            {
                FieldInfo fieldInfo => fieldInfo.FieldType,
                PropertyInfo propertyInfo => propertyInfo.PropertyType
            };
        }
        
        public static object Value(this MemberInfo info, object obj)
        {
            return info switch
            {
                FieldInfo fieldInfo => fieldInfo.GetValue(obj),
                PropertyInfo propertyInfo => propertyInfo.GetValue(obj)
            };
        }
    }
}