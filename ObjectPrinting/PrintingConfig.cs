using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        private static readonly Type[] FinalTypes =
            {typeof(int), typeof(double), typeof(float), typeof(string), typeof(DateTime), typeof(TimeSpan)};

        internal ConfigurationInfo ConfigurationInfo { get; }
        private List<List<object>> nestedSerializedObjects;
        private LinkedList<string> nestedNames;

        public PrintingConfig()
        {
            ConfigurationInfo = new ConfigurationInfo();
        }

        internal PrintingConfig(ConfigurationInfo configurationInfo)
        {
            ConfigurationInfo = configurationInfo;
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>() =>
            new PropertyPrintingConfig<TOwner, TPropType>(this);

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(
            Expression<Func<TOwner, TPropType>> memberSelector) =>
            new PropertyPrintingConfig<TOwner, TPropType>(this, memberSelector);

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector) =>
            new PrintingConfig<TOwner>(ConfigurationInfo.AddPropertyToExclude(memberSelector.GetObjectName()));

        public PrintingConfig<TOwner> Excluding<TPropType>() =>
            new PrintingConfig<TOwner>(ConfigurationInfo.AddTypeToExclude(typeof(TPropType)));

        public string PrintToString(TOwner obj)
        {
            nestedSerializedObjects = new List<List<object>> {new List<object>()};
            nestedNames = new LinkedList<string>();
            nestedNames.AddLast(typeof(TOwner).Name);
            return PrintToString(obj, 0);
        }

        private string PrintToString(object obj, int nestingLevel)
        {
            if (SerializedObjectsContains(nestingLevel, obj))
                return obj.GetType().Name + Environment.NewLine;

            if (obj == null)
                return "null" + Environment.NewLine;

            if (FinalTypes.Contains(obj.GetType()))
                return obj + Environment.NewLine;

            AddToSerializedObjects(nestingLevel, obj);
            var sb = new StringBuilder();

            sb.Append(obj is ICollection collection
                ? CollectionToString(collection, nestingLevel + 1)
                : MembersToString(obj, nestingLevel));

            return sb.ToString();
        }

        private StringBuilder CollectionToString(ICollection collection, int nestingLevel)
        {
            var sb = new StringBuilder();
            if (collection.Count != 0)
            {
                if (collection is IDictionary dictionary)
                    sb = DictionaryToString(dictionary, nestingLevel);
                else
                {
                    sb.Append(Environment.NewLine + new string('\t', nestingLevel) + '[' + Environment.NewLine);
                    foreach (var elem in collection)
                    {
                        sb.Append(new string('\t', nestingLevel));
                        sb.Append(PrintToString(elem, nestingLevel));
                    }

                    sb.Append(new string('\t', nestingLevel) + ']');
                }
            }
            else
                sb.Append("Empty");

            sb.Append(Environment.NewLine);
            return sb;
        }

        private StringBuilder DictionaryToString(IDictionary dictionary, int nestingLevel)
        {
            var sb = new StringBuilder();
            sb.Append(Environment.NewLine + new string('\t', nestingLevel) + '{' + Environment.NewLine);
            foreach (var key in dictionary.Keys)
            {
                sb.Append(new string('\t', nestingLevel));
                sb.Append($"{key}: ");
                sb.Append(PrintToString(dictionary[key], nestingLevel));
            }

            sb.Append(new string('\t', nestingLevel) + '}');
            return sb;
        }

        private StringBuilder MembersToString(object obj, int nestingLevel)
        {
            var sb = new StringBuilder();
            sb.AppendLine(new string(obj.GetType().Name));
            foreach (var propertyInfo in obj.GetType().GetProperties())
                sb.Append(MemberInfoToString(
                    new ValueInfo(propertyInfo.GetValue(obj), propertyInfo.PropertyType, propertyInfo.Name),
                    nestingLevel));
            var a = obj.GetType().GetFields();
            foreach (var fieldInfo in a.Where(f => f.Name != "Empty"))
                sb.Append(MemberInfoToString(
                    new ValueInfo(fieldInfo.GetValue(obj), fieldInfo.FieldType, fieldInfo.Name),
                    nestingLevel));

            nestedSerializedObjects[nestingLevel] = new List<object>();
            return sb;
        }

        private StringBuilder MemberInfoToString(ValueInfo valueInfo, int nestingLevel)
        {
            var sb = new StringBuilder();
            nestedNames.AddLast(valueInfo.Name);

            if (!ConfigurationInfo.ShouldExclude(valueInfo.Type, string.Join('.', nestedNames)))
            {
                sb.Append(new string('\t', nestingLevel + 1) + valueInfo.Name + " = ");
                var serialized = ConfigurationInfo.TryUseConfiguration(valueInfo, string.Join('.', nestedNames));
                if (serialized is null)
                {
                    serialized = PrintToString(valueInfo.Value, nestingLevel + 1);
                    sb.Append(serialized);
                }
                else
                {
                    sb.Append(serialized);
                    sb.Append(Environment.NewLine);
                }
            }

            nestedNames.RemoveLast();
            return sb;
        }

        private void AddToSerializedObjects(int nestingLevel, object obj)
        {
            if (nestedSerializedObjects.Count < nestingLevel + 1)
                nestedSerializedObjects.Add(new List<object>());
            nestedSerializedObjects[nestingLevel].Add(obj);
        }

        private bool SerializedObjectsContains(int nestingLevel, object obj)
        {
            for (var i = 0; i < nestingLevel; i++)
            {
                if (nestedSerializedObjects[i].Any(serialized => ReferenceEquals(serialized, obj)))
                    return true;
            }

            return false;
        }
    }
}