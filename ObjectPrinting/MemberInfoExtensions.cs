using System;
using System.Reflection;

namespace ObjectPrinting
{
    public static class MemberInfoExtensions
    {
        public static object GetValue(this MemberInfo memberInfo, object forObject)
        {
            return memberInfo switch
            {
                FieldInfo fieldInfo => fieldInfo.GetValue(forObject),
                PropertyInfo propertyInfo => propertyInfo.GetValue(forObject),
                _ => throw new ArgumentException("Not field or property")
            };
        } 
        
        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            return memberInfo switch
            {
                FieldInfo fieldInfo => fieldInfo.FieldType,
                PropertyInfo propertyInfo => propertyInfo.PropertyType,
                _ => throw new ArgumentException("Not field or property")
            };
        }
    }
}