using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Linq;
using ObjectPrinting.Printer;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        private const string RecursiveMessage = "[Recursive link]";
        private const string EqualsSymbol = " = ";

        protected readonly HashSet<Type> finalTypes = new HashSet<Type>
        {
            typeof(int), typeof(double), typeof(float), typeof(string),
            typeof(DateTime), typeof(TimeSpan)
        };

        internal Dictionary<Type, CultureInfo> Cultures = new Dictionary<Type, CultureInfo>();
        internal HashSet<MemberInfo> ExcludeMembers = new HashSet<MemberInfo>();
        internal HashSet<Type> ExcludingTypes = new HashSet<Type>();
        internal Dictionary<Type, Delegate> SerializeWaysByType = new Dictionary<Type, Delegate>();
        internal Dictionary<MemberInfo, Delegate> SerializeWaysByMembers = new Dictionary<MemberInfo, Delegate>();
        internal int? MaximumStringLength = null;

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0, new HashSet<object>());
        }

        public PrintingConfig<TOwner> Exclude<TProperty>()
        {
            ExcludingTypes.Add(typeof(TProperty));
            return this;
        }

        public PrintingConfig<TOwner> Exclude<T>(Expression<Func<TOwner, T>> selector)
        {
            ExcludeMembers.Add(GetMember(selector));
            return this;
        }

        public TypeEntity<TOwner, T> Select<T>()
        {
            return new TypeEntity<TOwner, T>(this, SerializeWaysByType);
        }

        public PropertyEntity<TOwner, T> Select<T>(Expression<Func<TOwner, T>> selector)
        {
            return new PropertyEntity<TOwner, T>(this, GetMember(selector), SerializeWaysByMembers);
        }

        private MemberInfo GetMember<T1, T2>(Expression<Func<T1, T2>> selector)
        {
            var memberExpression = selector.Body as MemberExpression;
            return memberExpression.Member;
        }

        private string PrintToString(object obj, int nestingLevel, HashSet<object> parents)
        {
            if (obj == null)
                return "null" + Environment.NewLine;

            var type = obj.GetType();
            if (finalTypes.Contains(type))
                return FormatFinalType(type, obj) + Environment.NewLine;

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            sb.Append(type.Name);
            if (parents.Contains(obj))
                return RecursiveMessage + Environment.NewLine;

            if (obj is IDictionary dictionary)
            {
                sb.Append(" ");
                sb.AppendLine(PrintDictionary(dictionary));
                return sb.ToString();
            }
            if (obj is IEnumerable enumerable)
            {
                sb.Append(" ");
                sb.AppendLine(PrintEnumerable(enumerable));
                return sb.ToString();
            }

            sb.AppendLine();
            foreach (var propertyInfo in type.GetProperties())
            {
                if (ExcludingTypes.Contains(propertyInfo.PropertyType))
                    continue;
                if (ExcludeMembers.Contains(propertyInfo))
                    continue;
                string result = null;
                var value = propertyInfo.GetValue(obj);
                if (SerializeWaysByMembers.ContainsKey(propertyInfo))
                    result = (string)SerializeWaysByMembers[propertyInfo].DynamicInvoke(value) + Environment.NewLine;
                else
                {
                    parents.Add(obj);
                    result = PrintToString(value, nestingLevel + 1, parents);
                    parents.Remove(obj);
                }
                sb.Append(identation + propertyInfo.Name + EqualsSymbol + result);
            }

            return sb.ToString();
        }

        private string PrintEnumerable(IEnumerable enumerable)
        {
            var builder = new StringBuilder();
            builder.Append("[ ");
            var first = true;
            foreach (var item in enumerable)
            {
                if (!first)
                    builder.Append(", ");
                builder.Append(item);
                first = false;
            }

            builder.Append(" ]");
            return builder.ToString();
        }

        private string PrintDictionary(IDictionary dictionary)
        {
            var builder = new StringBuilder();
            builder.Append("{ ");
            var first = true;
            var keyEnumerator = dictionary.Keys.GetEnumerator();
            var valueEnumerator = dictionary.Values.GetEnumerator();
            while (keyEnumerator.MoveNext() && valueEnumerator.MoveNext())
            {
                if (!first)
                    builder.Append(", ");
                builder.Append("{ " + keyEnumerator.Current + ": " + valueEnumerator.Current + " }");
                first = false;
            }

            builder.Append(" }");
            return builder.ToString();
        }

        private string FormatFinalType(Type type, object obj)
        {
            if (SerializeWaysByType.ContainsKey(type))
                return (string)SerializeWaysByType[type].DynamicInvoke(obj) + Environment.NewLine;
            if (Cultures.ContainsKey(type))
                return string.Format(Cultures[type], "{0}", obj) + Environment.NewLine;
            if (MaximumStringLength != null && obj is string value)
                return value.Substring(0, (int)MaximumStringLength);
            return obj.ToString();
        }
    }
}
