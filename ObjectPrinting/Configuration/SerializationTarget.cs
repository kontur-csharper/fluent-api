using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ObjectPrinting.Configuration
{
    /// <summary>
    /// Property or field to be serialized
    /// </summary>
    public class SerializationTarget
    {
        private readonly Func<object, object> valueGetter;

        public SerializationTarget(PropertyInfo propertyInfo)
        {
            valueGetter = propertyInfo.GetValue;
            MemberType = propertyInfo.PropertyType;
            OwnerType = propertyInfo.DeclaringType!;
            MemberName = propertyInfo.Name;
        }

        public SerializationTarget(FieldInfo fieldInfo)
        {
            valueGetter = fieldInfo.GetValue;
            MemberType = fieldInfo.FieldType;
            OwnerType = fieldInfo.DeclaringType!;
            MemberName = fieldInfo.Name;
        }

        public string MemberName { get; set; }
        public Type MemberType { get; set; }
        public Type OwnerType { get; set; }

        public object GetValue(object owner)
        {
            if (owner.GetType() != OwnerType)
                throw new ArgumentException($"Owner must have type {OwnerType}, but was {owner.GetType()}");
            return valueGetter.Invoke(owner);
        }

        public static IEnumerable<SerializationTarget> EnumerateFrom(Type type)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            return type.GetFields(bindingFlags)
                .Select(f => new SerializationTarget(f))
                .Union(type.GetProperties(bindingFlags).Where(p => p.GetIndexParameters().Length < 1)
                    .Select(p => new SerializationTarget(p))).ToArray();
        }
    }
}