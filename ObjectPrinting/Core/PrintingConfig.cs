using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ObjectPrinting.Extensions;
using ObjectPrinting.Interfaces;

namespace ObjectPrinting.Core
{
    public class PrintingConfig<TOwner> : IPrintingConfig
    {
        private readonly Dictionary<Type, Delegate> _alternativeSerializationByTypes;
        private readonly Dictionary<string, Delegate> _alternativeSerializationByFullName;
        private readonly HashSet<Type> _excludingTypes;
        private readonly HashSet<string> _excludingMemberNames;
        private readonly Dictionary<object, int> _visitedInstances;

        private static HashSet<Type> _finalTypes = new HashSet<Type>
        {
            typeof(string), typeof(DateTime), typeof(TimeSpan), typeof(decimal), typeof(Guid)
        };

        private const int MaxNestingLevel = 10;

        Dictionary<Type, Delegate> IPrintingConfig.AlternativeSerializationByTypes =>
            _alternativeSerializationByTypes;

        Dictionary<string, Delegate> IPrintingConfig.AlternativeSerializationByFullName
            => _alternativeSerializationByFullName;

        public PrintingConfig()
        {
            _alternativeSerializationByTypes = new Dictionary<Type, Delegate>();
            _alternativeSerializationByFullName = new Dictionary<string, Delegate>();
            _excludingTypes = new HashSet<Type>();
            _excludingMemberNames = new HashSet<string>();
            _visitedInstances = new Dictionary<object, int>();
        }

        public MemberPrintingConfig<TOwner, TMemberType> Printing<TMemberType>()
        {
            return new MemberPrintingConfig<TOwner, TMemberType>(this);
        }

        public MemberPrintingConfig<TOwner, TMemberType> Printing<TMemberType>(
            Expression<Func<TOwner, TMemberType>> memberSelector)
        {
            if (!(memberSelector.Body is MemberExpression memberExpression))
                throw new Exception("Expression type must be MemberExpression");
            return new MemberPrintingConfig<TOwner, TMemberType>(this,
                $"{memberExpression.GetFullNestedName()}.{memberExpression.Member.Name}");
        }

        public PrintingConfig<TOwner> Excluding<TMemberType>(Expression<Func<TOwner, TMemberType>> memberSelector)
        {
            if (!(memberSelector.Body is MemberExpression memberExpression))
                throw new Exception("Expression type must be MemberExpression");
            _excludingMemberNames.Add($"{memberExpression.GetFullNestedName()}.{memberExpression.Member.Name}");
            return this;
        }

        public PrintingConfig<TOwner> Excluding<TMemberType>()
        {
            _alternativeSerializationByTypes.Clear();
            _excludingTypes.Add(typeof(TMemberType));
            return this;
        }

        public string PrintToString(TOwner obj)
        {
            if (_visitedInstances.ContainsKey(obj))
                ClearTempStoragesBeforeNewInstanceSerialization();
            return PrintToString(obj, 0, "");
        }

        private void ClearTempStoragesBeforeNewInstanceSerialization()
        {
            _alternativeSerializationByTypes.Clear();
            _alternativeSerializationByFullName.Clear();
            _visitedInstances.Clear();
            _excludingMemberNames.Clear();
            _excludingTypes.Clear();
        }

        private string PrintToString(object obj, int nestingLevel, string partName)
        {
            if (nestingLevel > MaxNestingLevel)
                return "Nesting level exceeded\r\n";
            if (obj == null)
                return "null" + Environment.NewLine;
            var type = obj.GetType();
            if (IsFinalType(type))
                return obj + Environment.NewLine;
            if (IsContainsCycle(obj, nestingLevel))
                return $"Type = {type.Name}, Value = {obj} : found a cyclic link\r\n";
            if (!_visitedInstances.ContainsKey(obj))
                _visitedInstances[obj] = nestingLevel;
            return typeof(ICollection).IsAssignableFrom(type)
                ? GetSerializedCollection(obj, nestingLevel)
                : GetSerializedMembers(obj, nestingLevel, partName.Length == 0 ? type.Name : partName);
        }

        private bool IsContainsCycle(object currentObject, int currentNestingLevel)
        {
            return _visitedInstances.TryGetValue(currentObject, out var lastNestingLevel) &&
                   lastNestingLevel < currentNestingLevel;
        }

        private static bool IsFinalType(Type type) => _finalTypes.Contains(type) || type.IsPrimitive;

        private string GetSerializedCollection(object obj, int nestingLevel)
        {
            var indentation = new string('\t', nestingLevel + 1);
            var result = new StringBuilder().AppendLine(obj.GetType().Name);
            var collection = (IEnumerable) obj;
            foreach (var element in collection)
                result.Append(indentation + PrintToString(element, nestingLevel + 1, ""));
            return result.ToString();
        }

        private string GetSerializedMembers(object obj, int nestingLevel, string partName)
        {
            var serialized = new StringBuilder().AppendLine(obj.GetType().Name);
            foreach (var elementInfo in obj.GetElementsInfo(partName).Where(IsNotExcludingMember))
                serialized.Append(GetSerializedMember(obj, elementInfo, nestingLevel));
            return serialized.ToString();
        }

        private bool IsNotExcludingMember(ElementInfo elementInfo)
        {
            return !_excludingTypes.Contains(elementInfo.ElementType) &&
                   !_excludingMemberNames.Contains(elementInfo.FullName);
        }

        private string GetSerializedMember(object obj, ElementInfo elementInfo, int nestingLevel)
        {
            var member = new StringBuilder();
            var partResult = new string('\t', nestingLevel + 1) + elementInfo.Name + " = ";
            var elementValue = elementInfo.GetValue(obj);
            if (elementValue != null &&
                _alternativeSerializationByFullName.TryGetValue(elementInfo.FullName,
                    out var certainMemberSerialization))
            {
                var serializationResult = (string) certainMemberSerialization
                    .DynamicInvoke(elementValue);
                member.Append(partResult + serializationResult + Environment.NewLine);
            }
            else if (elementValue != null &&
                     _alternativeSerializationByTypes.TryGetValue(elementInfo.ElementType,
                         out var certainMemberTypeSerialization))
            {
                var serializationResult = (string) certainMemberTypeSerialization
                    .DynamicInvoke(elementValue);
                member.Append(partResult + serializationResult + Environment.NewLine);
            }
            else member.Append(partResult + PrintToString(elementValue, nestingLevel + 1, elementInfo.FullName));

            return member.ToString();
        }
    }
}