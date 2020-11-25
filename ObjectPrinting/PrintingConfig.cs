using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        private readonly Config config;
        private PropertyInfo selectedProperty;

        public PrintingConfig()
        {
            config = new Config();
        }

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, new List<object>(), 0);
        }

        public PrintingConfig<TOwner> TypeSerializer<TProperty>(Func<object, string> func)
        {
            config.TypesSerializer.Add(typeof(TProperty), func);
            return this;
        }

        public PrintingConfig<TOwner> Exclude<TProperty>()
        {
            config.ExcludedTypes.Add(typeof(TProperty));
            return this;
        }

        public SelectedProperty<TOwner, TProperty> Choose<TProperty>(Expression<Func<TOwner, TProperty>> selector)
        {
            selectedProperty = (PropertyInfo) ((MemberExpression) selector.Body).Member;
            return new SelectedProperty<TOwner, TProperty>(selectedProperty, this, config);
        }

        private string PrintToString(object obj, List<object> visited, int nestingLevel)
        {
            if (obj == null)
                return "null" + Environment.NewLine;

            var finalTypes = new[]
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan)
            };

            if (finalTypes.Contains(obj.GetType()))
                return obj + Environment.NewLine;

            if (visited.Contains(obj))
                return $"This element has been already added {obj}";

            visited.Add(obj);

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
            {
                if (config.IsExcluded(propertyInfo))
                    continue;

                sb.Append(identation + propertyInfo.Name + " = ");
                sb.Append(Serialize(propertyInfo, visited, obj, nestingLevel));
            }

            visited.Remove(obj);

            return sb.ToString();
        }

        private string Serialize(PropertyInfo propertyInfo, List<object> visited, object element, int nestingLevel)
        {
            if (propertyInfo.GetValue(element) is IDictionary dictionary)
                return SerializeDictionary(dictionary, visited, nestingLevel);
            if (propertyInfo.GetValue(element) is ICollection enumerable)
                return SerializeEnumerable(enumerable, visited, nestingLevel);
            return SerializeProperty(propertyInfo, element) ?? (SerializeType(propertyInfo, element)
                                                                ?? DefaultSerialization(propertyInfo, element,
                                                                    nestingLevel, visited));
        }

        private string SerializeEnumerable(IEnumerable enumerable, List<object> visited, int nestingLevel)
        {
            var sb = new StringBuilder();
            sb.Append(Environment.NewLine + new string('\t', nestingLevel + 1) + '[' + Environment.NewLine);

            foreach (var element in enumerable)
            {
                sb.Append(new string('\t', nestingLevel + 2));
                sb.Append(PrintToString(element, visited, nestingLevel));
            }

            sb.Append(new string('\t', nestingLevel + 1) + ']');

            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        private string SerializeDictionary(IDictionary dictionary, List<object> visited, int nestingLevel)
        {
            var sb = new StringBuilder();
            sb.Append(Environment.NewLine + new string('\t', nestingLevel + 1) + '{' + Environment.NewLine);

            foreach (var key in dictionary.Keys)
            {
                sb.Append(new string('\t', nestingLevel + 1));
                sb.Append(PrintToString(key, visited, nestingLevel + 2));
                sb.Append(": ");
                sb.Append(PrintToString(dictionary[key], visited, nestingLevel + 2));
            }

            sb.Append(new string('\t', nestingLevel + 1) + '}');

            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        private string SerializeProperty(PropertyInfo propertyInfo, object element)
        {
            var current = propertyInfo.GetValue(element);
            return config.IsSpecialSerialize(propertyInfo, current, out var result) ? result : null;
        }

        private string SerializeType(PropertyInfo propertyInfo, object element)
        {
            var current = propertyInfo.GetValue(element);
            return config.IsSpecialSerialize(propertyInfo.PropertyType, current, out var result) ? result : null;
        }

        private string DefaultSerialization(PropertyInfo propertyInfo, object element, int nestingLevel,
            List<object> visited)
        {
            return PrintToString(propertyInfo.GetValue(element), visited, nestingLevel + 1);
        }
    }
}