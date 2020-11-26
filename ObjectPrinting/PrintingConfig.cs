using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        private HashSet<Type> excludedTypes;
        private HashSet<MemberInfo> exludedMembers;
        internal Dictionary<Type, Delegate> typeMemberConfigs;
        internal Dictionary<MemberInfo, Delegate> nameMemberConfigs;
        private HashSet<object> serializingNow;

        public PrintingConfig()
        {
            excludedTypes = new HashSet<Type>();
            exludedMembers = new HashSet<MemberInfo>();
            typeMemberConfigs = new Dictionary<Type, Delegate>();
            nameMemberConfigs = new Dictionary<MemberInfo, Delegate>();
            serializingNow = new HashSet<object>(new ReferenceComparer());
        }

        public MemberPrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new MemberPrintingConfig<TOwner, TPropType>(this);
        }

        public MemberPrintingConfig<TOwner, TPropType> Printing<TPropType>(
            Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var member = ((MemberExpression) memberSelector.Body).Member;
            var propInfo = member as PropertyInfo;
            var fieldInfo = member as FieldInfo;
            if (propInfo == null && fieldInfo == null)
                throw new ArgumentException("Can't extract member from expression body");
            return new MemberPrintingConfig<TOwner, TPropType>(this, member);
        }

        public PrintingConfig<TOwner> Excluding<TMemberType>(Expression<Func<TOwner, TMemberType>> memberSelector)
        {
            var member = ((MemberExpression) memberSelector.Body).Member;
            var propInfo = member as PropertyInfo;
            var fieldInfo = member as FieldInfo;
            if (propInfo == null && fieldInfo == null)
                throw new ArgumentException("Can't extract member from expression body");
            exludedMembers.Add(member);
            return this;
        }

        internal PrintingConfig<TOwner> Excluding<TPropType>()
        {
            excludedTypes.Add(typeof(TPropType));
            return this;
        }

        public PrintingConfig<TOwner> TrimmedToLength(int maxLength)
        {
            if (maxLength < 0)
                throw new ArgumentException("maxLength must be non-negative");
            return Printing<string>()
                .Using(s => maxLength > s.Length ? s : s.Substring(0, maxLength));
        }

        public string PrintToString(TOwner obj)
        {
            serializingNow = new HashSet<object>(new ReferenceComparer());
            return PrintToString(obj, 0);
        }

        private string PrintCollectionToString(IEnumerable collection, int nestingLevel)
        {
            if (collection is IDictionary dictionary)
                return SerializeDictionary(dictionary, nestingLevel);

            var sb = new StringBuilder();
            sb.AppendLine();
            var identation = new string('\t', nestingLevel + 1);
            sb.Append(identation);
            sb.AppendLine("[");
            foreach (var item in collection)
            {
                sb.Append(identation);
                sb.Append(PrintToString(item, nestingLevel + 1));
            }

            sb.Append(identation);
            sb.AppendLine("]");
            return sb.ToString();
        }

        private string SerializeDictionary(IDictionary dictionary, int nestingLevel)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            var identation = new string('\t', nestingLevel + 1);
            sb.Append(identation);
            sb.AppendLine("[");
            foreach (DictionaryEntry keyValuePair in dictionary)
            {
                var key = keyValuePair.Key;
                var value = keyValuePair.Value;
                sb.Append(identation);
                sb.Append(PrintToString(key, nestingLevel + 1));
                sb.Length -= Environment.NewLine.Length;
                sb.Append(" : ");
                sb.Append(PrintToString(value, 0));
            }

            sb.Append(identation);
            sb.AppendLine("]");
            return sb.ToString();
        }

        private StringBuilder SerializeProperties(object obj, int nestingLevel)
        {
            var type = obj.GetType();
            var sb = new StringBuilder();
            foreach (var propertyInfo in type.GetProperties())
            {
                var identation = new string('\t', nestingLevel + 1);
                if (exludedMembers.Contains(propertyInfo))
                    continue;
                if (excludedTypes.Contains(propertyInfo.PropertyType))
                    continue;
                var value = propertyInfo.GetValue(obj);
                if (nameMemberConfigs.ContainsKey(propertyInfo))
                    value = nameMemberConfigs[propertyInfo].DynamicInvoke(value);
                else if (typeMemberConfigs.ContainsKey(propertyInfo.PropertyType))
                    value = typeMemberConfigs[propertyInfo.PropertyType].DynamicInvoke(value);
                sb.Append(identation + propertyInfo.Name + " = " +
                          PrintToString(value,
                              nestingLevel + 1));
            }

            return sb;
        }

        private StringBuilder SerializeFields(object obj, int nestingLevel)
        {
            var type = obj.GetType();
            var sb = new StringBuilder();
            foreach (var fieldInfo in type.GetFields())
            {
                var identation = new string('\t', nestingLevel + 1);
                if (exludedMembers.Contains(fieldInfo))
                    continue;
                if (excludedTypes.Contains(fieldInfo.FieldType))
                    continue;
                var value = fieldInfo.GetValue(obj);
                if (nameMemberConfigs.ContainsKey(fieldInfo))
                    value = nameMemberConfigs[fieldInfo].DynamicInvoke(value);
                else if (typeMemberConfigs.ContainsKey(fieldInfo.FieldType))
                    value = typeMemberConfigs[fieldInfo.FieldType].DynamicInvoke(value);
                sb.Append(identation + fieldInfo.Name + " = " +
                          PrintToString(value,
                              nestingLevel + 1));
            }

            return sb;
        }

        private string SerializeMembers(object obj, int nestingLevel)
        {
            var result = new StringBuilder(obj.GetType().Name + Environment.NewLine);
            result.Append(SerializeProperties(obj, nestingLevel));
            result.Append(SerializeFields(obj, nestingLevel));
            return result.ToString();
        }

        private string PrintToString(object obj, int nestingLevel)
        {
            if (obj == null)
                return "null" + Environment.NewLine;
            if (serializingNow.Contains(obj))
                return Environment.NewLine;
            serializingNow.Add(obj);
            var finalTypes = new[]
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan), typeof(Guid)
            };

            var objType = obj.GetType();
            if (finalTypes.Contains(objType))
            {
                serializingNow.Remove(obj);
                if (typeMemberConfigs.ContainsKey(objType))
                    return typeMemberConfigs[objType].DynamicInvoke(obj) + Environment.NewLine;
                return obj + Environment.NewLine;
            }

            string result;
            if (obj is IEnumerable collection)
            {
                result = PrintCollectionToString(collection, nestingLevel);
                serializingNow.Remove(obj);
                return result;
            }

            result = SerializeMembers(obj, nestingLevel);
            serializingNow.Remove(obj);
            return result;
        }
    }
}