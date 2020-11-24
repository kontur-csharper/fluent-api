using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ObjectPrinting
{
    public class ObjectPrinter<T>
    {
        private readonly PrintingConfig<T> config;
        private Dictionary<Type, List<object>> objectsCache;

        internal ObjectPrinter(PrintingConfig<T> config)
        {
            this.config = config;
        }

        public string PrintToString(T obj)
        {
            objectsCache = new Dictionary<Type, List<object>>();
            return PrintToString(obj, 0);
        }

        private string PrintToStringFinalType(object obj)
        {
            return obj.ToString();
        }

        private string PrintToStringCollection(ICollection collection, int nestingLevel)
        {
            var indentation = new string('\t', nestingLevel);
            var sb = new StringBuilder();
            sb.AppendLine($"{indentation}[");
            foreach (var element in collection)
                sb.AppendLine($"{indentation}\t{PrintToString(element, nestingLevel + 1)}");
            sb.Append($"{indentation}]");
            return sb.ToString();
        }

        private string PrintToString(object obj, int nestingLevel)
        {
            if (obj == null)
                return "null";

            var type = obj.GetType();

            if (config.IsFinal(type))
                return PrintToStringFinalType(obj);

            if (!objectsCache.ContainsKey(type))
                objectsCache[type] = new List<object>();
            int index;
            if ((index = objectsCache[type].IndexOf(obj)) != -1)
                return $"{type.Name} {index + 1}";
            objectsCache[type].Add(obj);

            var sb = new StringBuilder();
            sb.AppendLine($"{type.Name} {objectsCache[type].Count}");

            if (nestingLevel == config.maxSerializationDepth - 1)
                return sb.ToString();

            if (obj is ICollection)
            {
                sb.Append(PrintToStringCollection((ICollection) obj, nestingLevel));
                return sb.ToString();
            }

            var indentation = new string('\t', nestingLevel + 1);
            foreach (var propInfo in type.GetProperties())
            {
                if (config.IsExcluded(propInfo))
                    continue;
                if (config.TryGetCustomPrinter(propInfo, out var printer))
                    sb.AppendLine($"{indentation}{propInfo.Name} = {printer(propInfo.GetValue(obj))}");
                else
                    sb.AppendLine(
                        $"{indentation}{propInfo.Name} = {PrintToString(propInfo.GetValue(obj), nestingLevel + 1)}");
            }

            sb.Remove(sb.Length - Environment.NewLine.Length, Environment.NewLine.Length);
            return sb.ToString();
        }
    }

    public class ObjectPrinter
    {
        public static PrintingConfig<T> For<T>()
        {
            return new PrintingConfig<T>();
        }
    }
}