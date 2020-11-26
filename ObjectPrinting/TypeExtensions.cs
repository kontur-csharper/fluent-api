using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ObjectPrinting
{
    public static class TypeExtensions
    {
        public static IEnumerable<MemberInfo> GetFieldsAndProperties(this Type type)
        {
            return type.GetFields().Where(x => !x.IsStatic).Cast<MemberInfo>().Concat(type.GetProperties());
        }
    }
}