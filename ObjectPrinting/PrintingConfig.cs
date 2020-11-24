using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner> : IPrintingConfig<TOwner>
    {
        private readonly HashSet<object> appearedObjects;
        private readonly Type configType;

        private readonly HashSet<Type> finalTypes = new HashSet<Type>
        {
            typeof(int), typeof(double), typeof(float), typeof(string),
            typeof(DateTime), typeof(TimeSpan), typeof(Guid)
        };

        private readonly Dictionary<string, Delegate> propertySerialization;
        private readonly HashSet<string> propertyToExclude;
        private readonly Dictionary<Type, CultureInfo> typeCultureInfo;
        private readonly Dictionary<Type, Delegate> typeSerialization;
        private readonly HashSet<Type> typeToExclude;

        public PrintingConfig()
        {
            configType = typeof(TOwner);
            typeToExclude = new HashSet<Type>();
            typeSerialization = new Dictionary<Type, Delegate>();
            typeCultureInfo = new Dictionary<Type, CultureInfo>();
            propertySerialization = new Dictionary<string, Delegate>();
            propertyToExclude = new HashSet<string>();
            appearedObjects = new HashSet<object>();
        }

        public IPrintingConfig<TOwner> Exclude(params Type[] types)
        {
            types.ToList().ForEach(x => typeToExclude.Add(x));

            return this;
        }

        public IPrintingConfig<TOwner> Exclude<TProperty>(Expression<Func<TOwner, TProperty>> property)
        {
            var propertyName = ((MemberExpression) property.Body).Member.Name;
            propertyToExclude.Add(propertyName);

            return this;
        }

        public ITypePrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new TypePrintingConfig<TOwner, TPropType>(this);
        }

        public void AddSerialization(Type type, Delegate action)
        {
            if (typeSerialization.ContainsKey(type))
            {
                typeSerialization[type] = action;
                return;
            }

            typeSerialization.Add(type, action);
        }

        public void AddSerialization(string name, Delegate action)
        {
            if (propertySerialization.ContainsKey(name))
            {
                propertySerialization[name] = action;
                return;
            }

            propertySerialization.Add(name, action);
        }

        public IPropertyPrintingConfig<TProperty, TOwner> SelectProperty<TProperty>(
            Expression<Func<TOwner, TProperty>> property)
        {
            var propertyName = ((MemberExpression) property.Body).Member.Name;
            return new PropertyPrintingConfig<TProperty, TOwner>(this, propertyName);
        }

        public IPrintingConfig<TOwner> SetCultureInfo<T>(CultureInfo cultureInfo) where T : IFormattable
        {
            if (typeCultureInfo.ContainsKey(typeof(T)))
                typeCultureInfo[typeof(T)] = cultureInfo;

            typeCultureInfo.Add(typeof(T), cultureInfo);
            return this;
        }

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0, obj.GetType().Name, obj.GetType());
        }

        private string PrintToString(object obj, int nestingLevel, string propertyName, Type parentType)
        {
            if (obj == null)
                return "null" + Environment.NewLine;

            if (appearedObjects.Contains(obj))
                return "Fall in cycle" + Environment.NewLine;

            if (finalTypes.Contains(obj.GetType()))
                return SerializeProperty(obj, propertyName, parentType);

            appearedObjects.Add(obj);
            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);

            foreach (var propertyInfo in type.GetProperties())
            {
                if ((typeToExclude.Contains(propertyInfo.PropertyType) ||
                     propertyToExclude.Contains(propertyInfo.Name)) && type == configType)
                    continue;

                sb.Append(identation + propertyInfo.Name + " = ");
                var value = propertyInfo.GetValue(obj);
                if (value is ICollection collection)
                {
                    if (collection is IDictionary dictionary)
                    {
                        sb.Append(SerializeDictionary(dictionary, nestingLevel + 1));
                        continue;
                    }

                    sb.Append(SerializeCollection(collection, nestingLevel + 1));
                    continue;
                }

                sb.Append(PrintToString(value, nestingLevel + 1, propertyInfo.Name, type));
            }

            appearedObjects.Remove(obj);
            return sb.ToString();
        }

        private string SerializeProperty(object obj, string propertyName, Type parentType)
        {
            var culture = CultureInfo.CurrentCulture;
            if (typeCultureInfo.ContainsKey(obj.GetType()) && parentType == configType)
                culture = typeCultureInfo[obj.GetType()];

            if (propertySerialization.ContainsKey(propertyName) && parentType == configType)
                return string.Format(culture, "{0}", propertySerialization[propertyName].DynamicInvoke(obj)) +
                       Environment.NewLine;

            if (typeSerialization.ContainsKey(obj.GetType()) && parentType == configType)
                return string.Format(culture, "{0}", typeSerialization[obj.GetType()].DynamicInvoke(obj)) +
                       Environment.NewLine;

            return string.Format(culture, "{0}", obj) + Environment.NewLine;
        }

        private string SerializeCollection(ICollection collection, int nestingLevel)
        {
            var result = new StringBuilder();

            if (collection.Count == 0)
                return "empty" + Environment.NewLine;

            result.Append("{" + Environment.NewLine);
            foreach (var item in collection)
                result.Append(PrintToString(item, nestingLevel + 1, item.GetType().Name, item.GetType()));
            result.Append(new string('\t', nestingLevel) + "}" + Environment.NewLine);

            return result.ToString();
        }

        private string SerializeDictionary(IDictionary collection, int nestingLevel)
        {
            var result = new StringBuilder();

            if (collection.Count == 0)
                return "empty" + Environment.NewLine;

            result.Append("{" + Environment.NewLine);
            foreach (var key in collection.Keys)
            {
                result.Append(PrintToString(key, nestingLevel + 1, key.GetType().Name, key.GetType()));
                result.Append(" : ");
                result.Append(PrintToString(collection[key], nestingLevel + 1, key.GetType().Name, key.GetType()));
            }

            result.Append(new string('\t', nestingLevel) + "}" + Environment.NewLine);

            return result.ToString();
        }
    }
}