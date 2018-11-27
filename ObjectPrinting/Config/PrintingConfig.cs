﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace ObjectPrinting.Config
{
    public class PrintingConfig<TOwner>
    {
        private readonly Type[] finalTypes =
        {
            typeof(int), typeof(double), typeof(float), typeof(string),
            typeof(DateTime), typeof(TimeSpan)
        };

        private readonly HashSet<Type> typesToExclude;
        protected readonly Dictionary<Type, Func<object, string>> printingOverridedTypes;
        protected readonly Dictionary<Type, CultureInfo> cultureOverridedTypes;
        protected readonly Dictionary<PropertyInfo, Func<object, string>> printingOverridedProperties;


        public PrintingConfig()
        {
            typesToExclude = new HashSet<Type>();
            printingOverridedTypes = new Dictionary<Type, Func<object, string>>();
            cultureOverridedTypes = new Dictionary<Type, CultureInfo>();
            printingOverridedProperties = new Dictionary<PropertyInfo, Func<object, string>>();
        }

        public void OverrideTypePrinting<TPropType>(Func<TPropType, string> print)
        {
            var propType = typeof(TPropType);

            if (printingOverridedTypes.ContainsKey(propType))
                printingOverridedTypes[propType] = null;

            printingOverridedTypes[propType] = obj => print((TPropType) obj);
        }

        public void OverrideTypeCulture<TPropType>(CultureInfo culture)
        {
            var propType = typeof(TPropType);

            if (cultureOverridedTypes.ContainsKey(propType))
                cultureOverridedTypes[propType] = null;

            cultureOverridedTypes[propType] = culture;
        }

        public void OverridePropertyPrinting(PropertyInfo propertyInfo, Func<object, string> print)
        {
            if (printingOverridedProperties.ContainsKey(propertyInfo))
                printingOverridedProperties[propertyInfo] = null;

            printingOverridedProperties[propertyInfo] = print;
        }

        public PrintingConfig<TOwner> Excluding<TPropType>()
        {
            typesToExclude.Add(typeof(TPropType));

            return this;
        }

        public TypePrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new TypePrintingConfig<TOwner, TPropType>(this);
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(
            Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var propertyInfo = (PropertyInfo) ((MemberExpression) memberSelector.Body).Member;

            return new PropertyPrintingConfig<TOwner, TPropType>(this, propertyInfo);
        }

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            return this;
        }


        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, null, 0);
        }

        private string PrintToString(object obj, PropertyInfo propertyInfo, int nestingLevel)
        {
            if (obj == null)
                return "null";

            var type = obj.GetType();

            if (propertyInfo != null && printingOverridedProperties.ContainsKey(propertyInfo))
                return printingOverridedProperties[propertyInfo](obj);

            if (printingOverridedTypes.ContainsKey(type) && nestingLevel != 0)
                return printingOverridedTypes[type](obj);

            if (finalTypes.Contains(type))
            {
                if (cultureOverridedTypes.TryGetValue(type, out var cultureInfo))
                    return PrintWithCulture(obj, cultureInfo);

                return obj.ToString();
            }

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            sb.AppendLine(type.Name);

            foreach (var prop in type.GetProperties())
            {
                if (typesToExclude.Contains(prop.PropertyType))
                    continue;

                var propertyString = PrintToString(prop.GetValue(obj), prop, nestingLevel + 1);
                sb.Append(identation + prop.Name + " = " + propertyString + Environment.NewLine);
            }

            return sb.ToString();
        }

        private static string PrintWithCulture(object obj, CultureInfo cultureInfo)
        {
            var toStringMethod = obj.GetType().GetMethod("ToString", new[] {typeof(CultureInfo)});
            return toStringMethod?.Invoke(obj, new object[] {cultureInfo}).ToString();
        }
    }
}