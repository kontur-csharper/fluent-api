using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public static class Printer
    {
        private static readonly HashSet<Type> FinalTypes = new HashSet<Type>
        {
            typeof(int), typeof(double), typeof(float), typeof(string),
            typeof(DateTime), typeof(TimeSpan), typeof(Guid)
        };

        public static string PrintToString<TOwner>(TOwner obj,
            Func<Configurator<TOwner>, Configurator<TOwner>> configuratorFunc)
        {
            var config = configuratorFunc(new Configurator<TOwner>()).Build();
            return PrintToString(obj, 0, config);
        }

        private static string PrintToString(object obj, int nestingLevel, Config config)
        {
            if (obj == null)
            {
                return "null" + Environment.NewLine;
            }

            if (FinalTypes.Contains(obj.GetType()))
            {
                return obj + Environment.NewLine;
            }

            var indentation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            if (obj is IEnumerable collection)
            {
                return PrintCollectionToString(nestingLevel,
                    sb,
                    collection,
                    indentation,
                    config);
            }

            var type = obj.GetType();
            sb.AppendLine(type.Name);
            var props = type.GetProperties().Where(prop =>
                !config.TypesToExclude.Contains(prop.PropertyType) && !config.PropsToExclude.Contains(prop));
            foreach (var propertyInfo in props)
            {
                sb.Append(indentation + propertyInfo.Name + " = " +
                          PrintToString(propertyInfo,
                              obj,
                              nestingLevel,
                              config));
            }

            return sb.ToString();
        }

        private static string PrintCollectionToString(int nestingLevel, StringBuilder sb,
            IEnumerable collection,
            string indentation, Config config)
        {
            sb.AppendLine();
            foreach (var elem in collection)
            {
                sb.Append(indentation + PrintToString(elem, nestingLevel + 1, config));
            }

            return sb.ToString();
        }

        private static string PrintToString(PropertyInfo propertyInfo, object obj, int nestingLevel,
            Config config)
        {
            var value = propertyInfo.GetValue(obj) as dynamic;
            var type = propertyInfo.PropertyType;

            if (config.PropPrintingMethods.TryGetValue(propertyInfo, out var printProperty))
            {
                return printProperty.DynamicInvoke(value) + Environment.NewLine;
            }

            if (config.TypePrintingMethods.TryGetValue(type, out var printType))
            {
                return printType.DynamicInvoke(value) + Environment.NewLine;
            }

            if (config.TypePrintingCultureInfo.TryGetValue(type, out var cultureInfo))
            {
                return value?.ToString(cultureInfo) + Environment.NewLine;
            }

            return PrintToString(value, nestingLevel + 1, config);
        }
    }
}