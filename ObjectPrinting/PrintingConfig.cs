using System;
using System.Collections;
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
        private readonly Dictionary<string, List<Type>> excludedFields = new Dictionary<string, List<Type>>();
        private readonly Dictionary<Type, CultureInfo> cultures = new Dictionary<Type, CultureInfo>();
        private readonly Dictionary<Type, Delegate> serializationsForType = new Dictionary<Type, Delegate>();
        private readonly Dictionary<string, Dictionary<Type, Delegate>> serializationForProperty =
            new Dictionary<string, Dictionary<Type, Delegate>>();

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var name = ((MemberExpression) memberSelector.Body).Member.Name;
            var declaringType = ((MemberExpression) memberSelector.Body).Member.DeclaringType;
            if (excludedFields.ContainsKey(name))
                excludedFields[name].Add(declaringType);
            else
                excludedFields[name] = new List<Type>(){declaringType};
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

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(
            Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var name = ((MemberExpression) memberSelector.Body).Member.Name;
            var declaringType = ((MemberExpression) memberSelector.Body).Member.DeclaringType;
            return new PropertyPrintingConfig<TOwner, TPropType>(this, name, declaringType);
        }

        public string PrintToString(TOwner obj)
        {
            return obj is ICollection collection ? 
                PrintToStringICollectable(collection) : 
                PrintToString(obj, 0, new HashSet<object>());
        }
        
        public void AddSerializationForType<TPropType>(Type type, Func<TPropType, string> serializaton)
        {
            serializationsForType[type] = serializaton;
        }

        public void AddSerializationForProperty<TPropType>(string propertyName, Func<TPropType, string> serialization,
            Type declaringType)
        {
            if (!serializationForProperty.ContainsKey(propertyName))
            {
                serializationForProperty[propertyName] = new Dictionary<Type, Delegate>();
            }
            serializationForProperty[propertyName][declaringType] = serialization;
        }

        public void AddCultureForMember(Type type, CultureInfo culture)
        {
            cultures[type] = culture;
        }

        private readonly Type[] finalTypes = new[]
        {
            typeof(int), typeof(double), typeof(float), typeof(string),
            typeof(DateTime), typeof(TimeSpan)
        };

        private string PrintToString(object obj, int nestingLevel, HashSet<object> serializedObjects)
        {
            if (obj == null)
                return "null" + Environment.NewLine;

            if (MemberIsAlreadySerilized(obj, serializedObjects))
            {
                return "circle ref" + Environment.NewLine;
            }

            if (finalTypes.Contains(obj.GetType()))
            {
                return GetMemberWithCultureOrNull(obj) ?? obj + Environment.NewLine;
            }

            var copyList = new HashSet<object>(serializedObjects) {obj};
            return PrintObject(obj, nestingLevel, copyList);
        }

        private string PrintObject(object obj, int nestingLevel, HashSet<object> serilizedObjects)
        {
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
            {
                PrintMember(propertyInfo, obj, sb, nestingLevel, serilizedObjects);
            }

            foreach (var fieldInfo in type.GetFields())
            {
                PrintMember(fieldInfo, obj, sb, nestingLevel, serilizedObjects);
            }

            return sb.ToString();
        }

        private void PrintMember(MemberInfo info, object parent, StringBuilder sb, int nestingLevel,
            HashSet<object> serilizedObjects)
        {
            if (IsExcludedMember(info.Type(), info.Name, parent))
                return;
            
            var identation = new string('\t', nestingLevel + 1);
            sb.Append(identation + info.Name + " = ");
            
            if (MemberHasSpecialSerialization(info.Name, parent))
            {
                var serializator = serializationForProperty[info.Name][parent.GetType()];
                sb.Append(serializator.DynamicInvoke(info.Value(parent)) + Environment.NewLine);
                return;
            }
            
            if (serializationsForType.TryGetValue(info.Type(), out var func))
            {
                sb.Append(func.DynamicInvoke(info.Value(parent)) + Environment.NewLine);
                return;
            }

            sb.Append(PrintToString(info.Value(parent), nestingLevel + 1, serilizedObjects));
        }
        
        private string PrintToStringICollectable(IEnumerable collection)
        {
            var sb = new StringBuilder();
            var index = 0;
            foreach (var obj in collection)
            {
                sb.Append(index + ": ");
                sb.Append(PrintToString(obj, 1, new HashSet<object>()));
                index++;
            }

            return sb.ToString();
        }

        private bool IsExcludedMember(Type type, string name, object parent)
        {
            return excludedTypes.Contains(type) ||
                   (excludedFields.ContainsKey(name) && excludedFields[name].Contains(parent.GetType()));
        }

        private bool MemberHasSpecialSerialization(string name, object parent)
        {
            return serializationForProperty.ContainsKey(name) &&
                   serializationForProperty[name].ContainsKey(parent.GetType());
        }

        private string GetMemberWithCultureOrNull(object obj)
        {
            return cultures.TryGetValue(obj.GetType(), out var culture) 
                ? string.Format(culture, "{0}", obj) 
                : null;
        }

        private static bool MemberIsAlreadySerilized(object obj, HashSet<object> serializedObjects)
        {
            return serializedObjects.Any(serilizedObj => ReferenceEquals(obj, serilizedObj));
        }
    }
}