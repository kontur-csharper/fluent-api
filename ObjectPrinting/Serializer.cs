using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public class Serializer
    {
        private readonly SerializationConfig config;

        public Serializer(SerializationConfig serializationConfig) =>
            this.config = serializationConfig;

        public string Serialize(object obj) =>
            PrintToString(obj, 0);

        private string PrintToString(object obj, int nestingLevel)
        {
            if (nestingLevel > config.MaxNestingLevel)
                return "";

            if (obj == null)
                return "null";

            if (config.TypePrintingRules.TryGetValue(obj.GetType(), out var typePrintingRule))
                return typePrintingRule(obj);

            if (config.FinalTypes.Contains(obj.GetType()))
            {
                if (config.TypeCultures.TryGetValue(obj.GetType(), out var cultureInfo))
                    return ((dynamic) obj).ToString(cultureInfo);
                return obj.ToString();
            }

            return obj switch
            {
                IDictionary dictionary => SerializeDictionary(dictionary, nestingLevel),
                IEnumerable enumerable => SerializeIEnumerable(enumerable, nestingLevel),
                _ => SerializeMembers(obj, nestingLevel)
            };
        }

        private string SerializeDictionary(IDictionary dictionary, int nestingLevel)
        {
            var sb = new StringBuilder();
            var indentation = new string('\t', nestingLevel + 1);
            sb.AppendLine("{");
            foreach (var key in dictionary.Keys)
            {
                sb.Append(indentation + "{" + PrintToString(key, nestingLevel + 1) + Environment.NewLine
                          + indentation + ":" + PrintToString(dictionary[key], nestingLevel + 1) + "},"
                          + Environment.NewLine);
            }

            sb.Append(indentation + "}");
            
            var serializedIDictionary = sb.ToString();
            var lastIndexOf = serializedIDictionary.LastIndexOf(",", StringComparison.Ordinal);
            if (lastIndexOf != -1)
                return serializedIDictionary.Remove(lastIndexOf, 1);
            return serializedIDictionary;

            return sb.ToString();
        }

        private string SerializeIEnumerable(IEnumerable obj, int nestingLevel)
        {
            var indentation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            sb.AppendLine("[");
            foreach (var key in obj)
                sb.AppendLine(indentation + PrintToString(key, nestingLevel + 1) + ",");

            sb.Append(indentation + "]");
            var serializedIEnumerable = sb.ToString();

            var lastIndexOf = serializedIEnumerable.LastIndexOf(",", StringComparison.Ordinal);
            if (lastIndexOf != -1)
                return serializedIEnumerable.Remove(lastIndexOf, 1);
            return serializedIEnumerable;
        }

        private bool IsExcludedMember(MemberInfo memberInfo)
        {
            if ((memberInfo.MemberType & MemberTypes.Field) != 0)
                return config.ExcludedTypes.Contains((memberInfo as FieldInfo)?.FieldType) ||
                       config.ExcludedMembers.Contains(memberInfo);

            if ((memberInfo.MemberType & MemberTypes.Property) != 0)
                return config.ExcludedTypes.Contains((memberInfo as PropertyInfo)?.PropertyType) ||
                       config.ExcludedMembers.Contains(memberInfo);

            return false;
        }

        private string SerializeMembers(object obj, int nestingLevel)
        {
            var indentation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.Append(type.Name);
            foreach (var memberInfo in type.GetMembers()
                .Where(IsFieldOrProperty).Where(x => !IsExcludedMember(x)))
            {
                var value = GetValue(obj, memberInfo);

                if (value != null && value.Equals(obj))
                {
                    sb.Append(Environment.NewLine + indentation + memberInfo.Name + " = " +
                              obj.GetType().Name);
                    continue;
                }

                sb.Append(Environment.NewLine + indentation + memberInfo.Name + " = " +
                          SerializeValue(nestingLevel, memberInfo, value));
            }

            return sb.ToString();
        }

        private string SerializeValue(int nestingLevel, MemberInfo memberInfo, object value)
        {
            var valueString = "";

            if (config.MemberPrintingRules.TryGetValue(memberInfo, out var rule))
                valueString = rule(value);
            else if (config.MemberCultures.TryGetValue(memberInfo, out var cultureInfo))
                valueString = ((dynamic) value).ToString(cultureInfo);
            else
                valueString = (PrintToString(value, nestingLevel + 1));

            valueString = config.GlobalTrimToLength(valueString);

            if (config.MemberTrimToLength.TryGetValue(memberInfo, out var trimRule))
                return trimRule(valueString);
            return valueString;
        }

        private static bool IsFieldOrProperty(MemberInfo memberInfo) =>
            memberInfo.MemberType == MemberTypes.Field || memberInfo.MemberType == MemberTypes.Property;

        private static object GetValue(object obj, MemberInfo memberInfo)
        {
            if ((memberInfo.MemberType & MemberTypes.Field) != 0)
                return (memberInfo as FieldInfo)?.GetValue(obj);

            if ((memberInfo.MemberType & MemberTypes.Property) != 0)
                return (memberInfo as PropertyInfo)?.GetValue(obj);

            return null;
        }
    }
}