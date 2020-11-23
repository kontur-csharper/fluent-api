using System;
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
        private readonly List<Type> excludedTypes = new List<Type>();
        private readonly List<Delegate> excludedFields = new List<Delegate>();
        public readonly Dictionary<Type, CultureInfo> Cultures = new Dictionary<Type, CultureInfo>();
        public readonly Dictionary<Type, Delegate> Serializations = new Dictionary<Type, Delegate>();
        public readonly Dictionary<Delegate, int> TrimForStringProperties = new Dictionary<Delegate, int>();

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            excludedFields.Add(memberSelector.Compile());
            return this;
        }
        internal PrintingConfig<TOwner> Excluding<TPropType>()
        {
            excludedTypes.Add(typeof(TPropType));
            return this;
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(this);
        }
        
        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(this);
        }
        
        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        private string PrintToString(object obj, int nestingLevel)
        {
            if (obj == null)
                return "null" + Environment.NewLine;

            var finalTypes = new[]
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan)
            };
            if (Serializations.TryGetValue(obj.GetType(), out var func))
            {
                return func.DynamicInvoke(obj) + Environment.NewLine;
            }
            if (finalTypes.Contains(obj.GetType()))
            {
                if (Cultures.TryGetValue(obj.GetType(), out var culture))
                {
                    switch (obj)
                    {
                        case int i:
                            return i.ToString(culture) + Environment.NewLine;
                        case double d:
                            return d.ToString(culture) + Environment.NewLine;
                        case float f:
                            return f.ToString(culture) + Environment.NewLine;
                        
                    }
                }
                return obj + Environment.NewLine;
            }

            return PrintObject(obj, nestingLevel);
        }

        private string PrintObject(object obj, int nestingLevel)
        {
            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
            {
                if (excludedTypes.Contains(propertyInfo.PropertyType))
                {
                    continue;
                }

                var isExcluded = false;
                foreach (var func in excludedFields)
                {
                    if (func.DynamicInvoke(obj) == propertyInfo.GetValue(obj))
                    {
                        isExcluded = true;
                        break;
                    }
                }

                if (!isExcluded)
                {
                    sb.Append(identation + propertyInfo.Name + " = ");
                    sb.Append(PrintToString(propertyInfo.GetValue(obj),
                        nestingLevel + 1));
                }
            }
            return sb.ToString();
        }
    }
}