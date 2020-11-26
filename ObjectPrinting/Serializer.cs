using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace ObjectPrinting
{
    public class Serializer<T>
    {
        private readonly PrintingConfig<T> config;
        private readonly Stack<object> serializedObjects = new Stack<object>();

        public Serializer(PrintingConfig<T> config)
        {
            this.config = config;
        }

        public string Serialize(T obj)
        {
            return PrintToString(obj, 0);
        }

        private string PrintToString(object obj, int nestingLevel)
        {
            if (obj == null)
                return $"null{Environment.NewLine}";
            if (config.IsTypeFinal(obj.GetType()))
                return $"{obj}{Environment.NewLine}";
            if (serializedObjects.Contains(obj))
                throw new SerializationException("Circular reference");
            serializedObjects.Push(obj);
            var sb = new StringBuilder();
            sb.AppendLine(obj.GetType().Name);
            switch (obj)
            {
                case IDictionary dictionary:
                    sb.Append(SerializeDictionary(dictionary, nestingLevel));
                    break;
                case IEnumerable enumerable:
                    sb.Append(SerializeCollection(enumerable, nestingLevel));
                    break;
                default:
                    sb.Append(SerializeObject(obj, nestingLevel));
                    break;
            }

            serializedObjects.Pop();
            return sb.ToString();
        }

        private string SerializeCollection(IEnumerable collection, int nestingLevel)
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetIndentation(nestingLevel) + "[");
            foreach (var e in collection)
                sb.Append(GetIndentation(nestingLevel + 1)
                          + PrintToString(e, nestingLevel + 1));
            sb.AppendLine(GetIndentation(nestingLevel) + "]");
            return sb.ToString();
        }

        private string SerializeDictionary(IDictionary dictionary, int nestingLevel)
        {
            var sb = new StringBuilder();
            sb.AppendLine(GetIndentation(nestingLevel) + "[");
            foreach (DictionaryEntry e in dictionary)
            {
                sb.AppendLine(GetIndentation(nestingLevel + 1) + "{");
                sb.Append(GetIndentation(nestingLevel + 2) +
                          $"Key = {PrintToString(e.Key, nestingLevel + 2)}");
                sb.Append(GetIndentation(nestingLevel + 2) +
                          $"Value = {PrintToString(e.Value, nestingLevel + 2)}");
                sb.AppendLine(GetIndentation(nestingLevel + 1) + "}");
            }

            sb.AppendLine(GetIndentation(nestingLevel) + "]");
            return sb.ToString();
        }

        private string SerializeObject(
            object obj,
            int nestingLevel)
        {
            var sb = new StringBuilder();
            foreach (var memberInfo in obj.GetType()
                .GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.IsPropertyOrField())
                .Where(config.IsMemberNotExcluded))
            {
                var memberValue = memberInfo.GetValue(obj);
                var toPrint = GetSerializedOrInputValue(memberInfo, memberValue);
                if (config.TryGetMemberLength(memberInfo, out var maxLength))
                    toPrint = toPrint?.ToString().Substring(0, maxLength);
                var serializedObject = PrintToString(toPrint, nestingLevel + 1);
                sb.Append($"{GetIndentation(nestingLevel + 1)}{memberInfo.Name} = {serializedObject}");
            }

            return sb.ToString();
        }

        private object GetSerializedOrInputValue(MemberInfo memberInfo, object value)
        {
            return config.TryGetSerializer(memberInfo, out var serializer)
                ? serializer(value)
                : value;
        }

        private static string GetIndentation(int nestingLevel)
        {
            return new string('\t', nestingLevel);
        }
    }
}