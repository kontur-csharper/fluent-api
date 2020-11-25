using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ObjectPrinting.Contracts;

namespace ObjectPrinting
{
    public class Serializer
    {
        private IPrintingConfig Config { get; }

        private readonly Type[] finalTypes =
        {
            typeof(int),
            typeof(double),
            typeof(float),
            typeof(string),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(Guid)
        };

        private readonly HashSet<object> visitedMembers = new HashSet<object>();

        public Serializer(IPrintingConfig printingConfig) => Config = printingConfig;

        public string PrintToString(object obj, int nestingLevel)
        {
            if (obj is null)
                return "null" + Environment.NewLine;

            if (finalTypes.Contains(obj.GetType()))
                return obj + Environment.NewLine;
            if (obj is IEnumerable collection)
                return PrintCollection(collection, nestingLevel);

            if (!visitedMembers.Add(obj))
                return "Infinite loop" + Environment.NewLine;

            var sb = new StringBuilder();

            sb.AppendLine(obj.GetType().Name);
            PrintingFields(obj, nestingLevel + 1, sb);
            PrintingProperties(obj, nestingLevel + 1, sb);
            
            visitedMembers.Remove(obj);
            return sb.ToString();
        }

        private void PrintingProperties(object obj, int nestingLevel, StringBuilder sb)
        {
            var nonExcludedProperties = obj.GetType().GetProperties().Where(p => !Config.PrintExcluder.IsExclude(p));
            foreach (var property in nonExcludedProperties)
            {
                var indentation = GetIdentationFor(nestingLevel);
                var propertyValue = PrintProperty(property.GetValue(obj), property, nestingLevel);
                sb.Append($"{indentation}{property.Name} = {propertyValue}");
            }
        }

        private void PrintingFields(object obj, int nestingLevel, StringBuilder sb)
        {
            var nonExcludedFields = obj.GetType().GetFields().Where(p => !Config.PrintExcluder.IsExclude(p));
            foreach (var field in nonExcludedFields)
            {
                var indentation = GetIdentationFor(nestingLevel);
                var fieldValue = PrintField(field.GetValue(obj), field, nestingLevel);
                sb.Append($"{indentation}{field.Name} = {fieldValue}");
            }
        }

        private string PrintField(object value, FieldInfo field, int nestingLevel)
        {
            if (Config.AlternativePrinter.HasAlternativePrintFor(field))
                return Config.AlternativePrinter.SerializeField(field, value) + Environment.NewLine;
            if (Config.AlternativePrinter.HasAlternativePrintFor(value))
                return Config.AlternativePrinter.SerializeValue(value) + Environment.NewLine;
            return PrintToString(value, nestingLevel);
        }

        private string PrintProperty(object value, PropertyInfo property, int nestingLevel)
        {
            if (Config.AlternativePrinter.HasAlternativePrintFor(property))
                return Config.AlternativePrinter.SerializeProperty(property, value) + Environment.NewLine;
            if (Config.AlternativePrinter.HasAlternativePrintFor(value))
                return Config.AlternativePrinter.SerializeValue(value) + Environment.NewLine;
            return PrintToString(value, nestingLevel);
        }

        private string PrintCollection(IEnumerable collection, int nestingLevel)
        {
            var sb = new StringBuilder();
            var newCollection = from object element in collection
                select GetIdentationFor(nestingLevel + 1) + PrintToString(element, nestingLevel + 1);

            return sb
                .AppendLine(collection.GetType().Name)
                .AppendLine(GetIdentationFor(nestingLevel) + '{')
                .AppendJoin(Environment.NewLine, newCollection)
                .AppendLine(GetIdentationFor(nestingLevel) + '}')
                .ToString();
        }

        private static string GetIdentationFor(int nestingLevel) => new string('\t', nestingLevel);
    }
}