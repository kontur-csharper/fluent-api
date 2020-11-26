using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public class Serializer<TOwner>
    {
        private readonly Type[] _finalTypes =
        {
            typeof(int), typeof(double), typeof(float), typeof(string),
            typeof(DateTime), typeof(TimeSpan), typeof(char), typeof(Guid), typeof(bool)
        };

        private readonly Type _mainType;
        private readonly HashSet<object> _serializedObjects;
        private readonly IPrintingConfig<TOwner> _printingConfig;

        public Serializer(PrintingConfig<TOwner> printingConfig)
        {
            _printingConfig = printingConfig;
            _mainType = typeof(TOwner);
            _serializedObjects = new HashSet<object>();
        }

        public string Serialize(object obj, int nestingLevel = 0)
        {
            if (obj is null)
                return "null" + Environment.NewLine;

            var objType = obj.GetType();

            if (IsFinalType(objType) || objType.IsPrimitive)
                return obj + Environment.NewLine;

            if (IsLoop(obj))
                return $"{objType.Name}. Here's a loop." + Environment.NewLine;

            _serializedObjects.Add(obj);

            return obj is ICollection enumerable
                ? SerializeEnumerable(enumerable, nestingLevel + 1)
                : SerializeMember(obj, nestingLevel);
        }

        private string SerializeMember(object obj, int nestingLevel)
        {
            var objType = obj.GetType();
            var indentation = GetIndentation('\t', nestingLevel + 1);

            var sb = new StringBuilder();

            sb.AppendLine(objType.Name);

            foreach (var memberInfo in objType.GetFieldsAndProperties())
            {
                if (IsExclude(memberInfo))
                    continue;

                sb.Append($"{indentation}{memberInfo.Name} = ");

                var value = memberInfo.GetValue(obj);
                var serializedObject = TryGetSpecialSerialize(memberInfo, out var specSerialize)
                    ? specSerialize.DynamicInvoke(value) + Environment.NewLine
                    : $"{Serialize(value, nestingLevel + 1)}";

                sb.Append(serializedObject);
            }

            return sb.ToString();
        }

        private string SerializeEnumerable(IEnumerable enumerable, int nestingLevel)
        {
            var sb = new StringBuilder();

            sb.AppendLine(enumerable.GetType().Name);
            sb.Append($"{GetIndentation('\t', nestingLevel)}[{Environment.NewLine}");

            foreach (var element in enumerable)
                sb.Append($"{GetIndentation('\t', nestingLevel + 1)}{Serialize(element, nestingLevel + 1)}");

            sb.Append($"{GetIndentation('\t', nestingLevel)}]{Environment.NewLine}");

            return sb.ToString();
        }

        private bool IsLoop(object obj)
        {
            return _serializedObjects.Contains(obj);
        }

        private bool TryGetSpecialSerialize(MemberInfo member, out Delegate result)
        {
            result = default;
            return IsMainType(member.DeclaringType) &&
                   (_printingConfig.SpecialSerializationTypes.TryGetValue(member.GetValueType(), out result) ||
                    _printingConfig.SpecialSerializationMembers.TryGetValue(member, out result));

        }

        private bool IsFinalType(Type objType)
        {
            return _finalTypes.Contains(objType);
        }

        private bool IsMainType(Type type)
        {
            return type == _mainType;
        }

        private static string GetIndentation(char indentationSymbol, int nestingLevel)
        {
            return new string(indentationSymbol, nestingLevel);
        }

        private bool IsExclude(MemberInfo memberInfo)
        {
            return _printingConfig.ExcludedMembers.Contains(memberInfo) ||
                   (IsMainType(memberInfo.DeclaringType) &&
                    _printingConfig.ExcludedTypes.Contains(memberInfo.GetValueType()));
        }
    }
}
