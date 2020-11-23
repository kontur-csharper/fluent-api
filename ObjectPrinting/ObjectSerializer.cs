using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ObjectPrinting.Config;

namespace ObjectPrinting
{
    public class ObjectSerializer<TOwner>
    {
        private readonly IPrintingConfig<TOwner> config;
        private readonly HashSet<object> visited = new HashSet<object>();

        private static readonly IEnumerable<Type> finalTypes = new[] {
            typeof(int), typeof(double), typeof(float), typeof(string),
            typeof(DateTime), typeof(TimeSpan)
        };

        public ObjectSerializer(IPrintingConfig<TOwner> printingConfig)
        {
            config = printingConfig;
        }

        public string Serialize(object obj, int nestingLevel)
        {
            if (obj == null)
                return "null" + Environment.NewLine;

            var type = obj.GetType();
            if (visited.Contains(obj))
                return "cycle" + Environment.NewLine;

            if (config.TypeToSerializer.TryGetValue(type, out var serializer))
                return serializer.DynamicInvoke(obj) + Environment.NewLine;

            if (finalTypes.Contains(type))
                return obj + Environment.NewLine;

            if (obj is IEnumerable enumerable)
                return PrintCollection(enumerable, nestingLevel) + Environment.NewLine;

            visited.Add(obj);

            var indent = GetIndent(nestingLevel);
            var serializedObj = new StringBuilder()
                .AppendLine(type.Name)
                .Append(SerializeProperties(obj, type, indent, nestingLevel))
                .Append(SerializeFields(obj, type, indent, nestingLevel));
            return serializedObj.ToString();
        }

        private static string GetIndent(int nestingLevel)
        {
            return new string('\t', nestingLevel + 1);
        }

        private StringBuilder SerializeProperties(object obj, Type type, string indent, int nestingLevel)
        {
            var sb = new StringBuilder();

            foreach (var propInfo in type.GetProperties()
                .Where(info => !config.ExcludedProperties.Contains(info) &&
                               !config.ExcludedTypes.Contains(info.PropertyType)))
                sb.Append(indent)
                    .Append(propInfo.Name)
                    .Append(" = ")
                    .Append(PrintProperty(propInfo.GetValue(obj), propInfo, nestingLevel + 1));

            return sb;
        }

        private StringBuilder SerializeFields(object obj, Type type, string indent, int nestingLevel)
        {
            var sb = new StringBuilder();

            foreach (var fieldInfo in type.GetFields()
                .Where(info => !config.ExcludedFields.Contains(info) &&
                               !config.ExcludedTypes.Contains(info.FieldType)))
                sb.Append(indent)
                    .Append(fieldInfo.Name)
                    .Append(" = ")
                    .Append(PrintField(fieldInfo.GetValue(obj), fieldInfo, nestingLevel + 1));

            return sb;
        }

        private string PrintProperty(object value, PropertyInfo propertyInfo, int nestingLevel)
        {
            return config.PropertyToSerializer.TryGetValue(propertyInfo, out var serializer) ?
                 serializer.DynamicInvoke(value) + Environment.NewLine :
                 Serialize(value, nestingLevel);
        }

        private string PrintField(object value, FieldInfo fieldInfo, int nestingLevel)
        {
            return config.FieldToSerializer.TryGetValue(fieldInfo, out var serializer) ?
                 serializer.DynamicInvoke(value) + Environment.NewLine :
                 Serialize(value, nestingLevel);
        }

        private string PrintCollection(IEnumerable collection, int nestingLevel)
        {
            var serializedObj = new StringBuilder("[" + Environment.NewLine);
            foreach (var el in collection)
                serializedObj
                    .Append(GetIndent(nestingLevel + 1))
                    .Append(Serialize(el, nestingLevel + 1))
                    .Append(GetIndent(nestingLevel + 2))
                    .Append("," + Environment.NewLine);

            return serializedObj
                .Remove(serializedObj.Length - 3, 3)
                .Append("]")
                .ToString();
        }
    }
}
