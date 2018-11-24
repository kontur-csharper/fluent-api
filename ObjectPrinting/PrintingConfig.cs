﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public interface IPrintingConfig<TOwner>
    {
        Dictionary<Type, Expression<Func<object, string>>> Printers { get; }
        Dictionary<Type, CultureInfo> CultureInfoForTypes { get; }
    }
    public class PrintingConfig<TOwner> : IPrintingConfig<TOwner>
    {
        private List<Type> excludedTypes = new List<Type>();
        private List<string> excluded = new List<string>();
        private Dictionary<Type, CultureInfo> cultureInfoForTypes = new Dictionary<Type, CultureInfo>();
        private Dictionary<Type, Expression<Func<object, string>>> printers = new Dictionary<Type, Expression<Func<object, string>>>();


        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(this);
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(this);
        }

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            //TODO add excluding by name
            excluded.Add(((MemberExpression)memberSelector.Body).Member.Name);
            return this;
        }

        internal PrintingConfig<TOwner> Excluding<TPropType>()
        {
            excludedTypes.Add(typeof(TPropType));
            return this;
        }

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        private string GetPropertyPrintingValue(PropertyInfo propertyInfo, object obj, int nestingLevel)
        {
            if (excludedTypes.Contains(propertyInfo.PropertyType))
                return string.Empty;

            if (excluded.Contains(propertyInfo.Name))
                return string.Empty;

            if (printers.ContainsKey(propertyInfo.PropertyType))
                return propertyInfo.Name + " = " + printers[propertyInfo.PropertyType].Compile().Invoke(propertyInfo.GetValue(obj));

            if (cultureInfoForTypes.ContainsKey(propertyInfo.PropertyType))
            {
                var type = propertyInfo.PropertyType;
                if (type == typeof(int))
                    return propertyInfo.Name + " = " + ((int)propertyInfo.GetValue(obj)).ToString(cultureInfoForTypes[propertyInfo.PropertyType]);
                if (type == typeof(double))
                    return propertyInfo.Name + " = " + ((double)propertyInfo.GetValue(obj)).ToString(cultureInfoForTypes[propertyInfo.PropertyType]);
                if (type == typeof(long))
                    return propertyInfo.Name + " = " + ((long)propertyInfo.GetValue(obj)).ToString(cultureInfoForTypes[propertyInfo.PropertyType]);

            }
            //TODO Add trimming of strings 

            //TODO Add excluding of types 

            //TODO Add alternative way to print 

            //TODO apply configurations
            return propertyInfo.Name + " = " +
                   PrintToString(propertyInfo.GetValue(obj), nestingLevel + 1);
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
            if (finalTypes.Contains(obj.GetType()))
                return obj + Environment.NewLine;

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
            {
                sb.Append(identation + GetPropertyPrintingValue(propertyInfo, obj, nestingLevel));
            }
            return sb.ToString();
        }

        Dictionary<Type, Expression<Func<object, string>>> IPrintingConfig<TOwner>.Printers => printers;
        Dictionary<Type, CultureInfo> IPrintingConfig<TOwner>.CultureInfoForTypes => cultureInfoForTypes;
    }
}