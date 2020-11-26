using System.Collections.Generic;
using System.Reflection;
using ObjectPrinting.Core;

namespace ObjectPrinting.Extensions
{
    public static class ObjectExtension
    {
        public static IEnumerable<ElementInfo> GetElementsInfo(this object obj, string fullName)
        {
            var objectType = obj.GetType();
            foreach (var propertyInfo in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                yield return new ElementInfo(propertyInfo, fullName);
            foreach (var fieldInfo in objectType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                yield return new ElementInfo(fieldInfo, fullName);
        }
    }
}