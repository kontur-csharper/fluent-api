using System;
using System.Reflection;

namespace ObjectPrinting
{
    public static class MemberInfoExtensions
    {
        public static bool IsPropertyOrField(this MemberInfo memberInfo)
        {
            return memberInfo.MemberType == MemberTypes.Field
                   || memberInfo.MemberType == MemberTypes.Property;
        }

        public static object GetValue(this MemberInfo memberInfo, object obj)
        {
            return memberInfo.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo) memberInfo).GetValue(obj),
                MemberTypes.Property => ((PropertyInfo) memberInfo).GetValue(obj),
                _ => throw new InvalidOperationException()
            };
        }

        public static Type GetValueType(this MemberInfo memberInfo)
        {
            return memberInfo.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo) memberInfo).FieldType,
                MemberTypes.Property => ((PropertyInfo) memberInfo).PropertyType,
                _ => throw new InvalidOperationException()
            };
        }
    }
}