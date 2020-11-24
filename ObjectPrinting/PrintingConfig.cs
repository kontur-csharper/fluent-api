using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner> : IPrintingConfig<TOwner>
    {
        private readonly HashSet<object> parentObjects = new HashSet<object>();
        private ImmutableHashSet<MemberInfo> excludedMembers = ImmutableHashSet<MemberInfo>.Empty;
        private ImmutableHashSet<Type> excludedTypes = ImmutableHashSet<Type>.Empty;

        private ImmutableDictionary<MemberInfo, Func<object, string>> MemberSerializations =
            ImmutableDictionary<MemberInfo, Func<object, string>>.Empty;

        private ImmutableDictionary<Type, Func<object, string>> typesSerialization =
            ImmutableDictionary<Type, Func<object, string>>.Empty;

        ImmutableDictionary<Type, Func<object, string>> IPrintingConfig<TOwner>.TypesSerialization
        {
            get => typesSerialization;
            set => typesSerialization = value;
        }

        ImmutableDictionary<MemberInfo, Func<object, string>> IPrintingConfig<TOwner>.PropsSerialization
        {
            get => MemberSerializations;
            set => MemberSerializations = value;
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return PropertyPrintingConfig<TOwner, TPropType>.For<TPropType>(this);
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(
            Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var selectedProp = (PropertyInfo) ((MemberExpression) memberSelector.Body).Member;
            return PropertyPrintingConfig<TOwner, TPropType>.For(this, selectedProp);
        }

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var selectedProp = (PropertyInfo) ((MemberExpression) memberSelector.Body).Member;
            excludedMembers = excludedMembers.Add(selectedProp);
            return this;
        }

        public PrintingConfig<TOwner> Excluding<TPropType>()
        {
            excludedTypes = excludedTypes.Add(typeof(TPropType));
            return this;
        }

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        public string PrintToString(ICollection<TOwner> objCollection)
        {
            var sb = new StringBuilder();
            sb.AppendLine(objCollection.GetType().Name);

            foreach (var obj in objCollection)
                sb.AppendLine(PrintToString(obj, 1));

            return sb.ToString();
        }

        public string PrintToString<TKey>(IDictionary<TKey, TOwner> objDictionary)
        {
            var sb = new StringBuilder();
            sb.AppendLine(objDictionary.GetType().Name);

            foreach (var (key, value) in objDictionary)
                sb.AppendLine($"{key} : {PrintToString(value, 1)}");

            return sb.ToString();
        }

        private string PrintToString(object obj, int nestingLevel)
        {
            if (obj == null)
                return "null";

            var finalTypes = new[]
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan), typeof(Guid)
            };

            if (finalTypes.Contains(obj.GetType()) || typesSerialization.ContainsKey(obj.GetType()))
            {
                var valueToPrint = typesSerialization.ContainsKey(obj.GetType())
                    ? typesSerialization[obj.GetType()](obj)
                    : obj;

                return valueToPrint.ToString();
            }

            parentObjects.Add(obj);
            var identation = new string('\t', nestingLevel);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(identation + type.Name);
            identation += '\t';

            var membersToPrint = GetMembersToPrint(obj, nestingLevel, type);
            
            foreach (var (key, value) in membersToPrint)
                sb.AppendLine($"{identation}{key} = {value}");

            return sb.ToString();
        }

        private Dictionary<string, string> GetMembersToPrint(object obj, int nestingLevel, Type type)
        {
            var membersToPrint = new Dictionary<string, string>();
            
            foreach (var propInfo in type.GetProperties())
            {
                if(excludedMembers.Contains(propInfo) || excludedTypes.Contains(propInfo.PropertyType))
                    continue;

                membersToPrint[propInfo.Name] = GetValueToPrint(obj, nestingLevel, propInfo);
            }
            
            foreach (var fieldInfo in type.GetFields())
            {
                if(excludedMembers.Contains(fieldInfo) || excludedTypes.Contains(fieldInfo.FieldType))
                    continue;

                membersToPrint[fieldInfo.Name] = GetValueToPrint(obj, nestingLevel, fieldInfo);
            }
            
            return membersToPrint;
        }


        private string GetValueToPrint(object obj, int nestingLevel, PropertyInfo propertyInfo)
        {
            return GetValueToPrint(propertyInfo, propertyInfo.GetValue(obj), nestingLevel);
        }

        private string GetValueToPrint(object obj, int nestingLevel, FieldInfo fieldInfo)
        {
            return GetValueToPrint(fieldInfo, fieldInfo.GetValue(obj), nestingLevel);
        }

        private string GetValueToPrint(MemberInfo memberInfo, object memberValue, int nestingLevel)
        {
            string valueToPrint;
            if (parentObjects.Contains(memberValue))
                valueToPrint = "<parent>";
            else
                valueToPrint = MemberSerializations.ContainsKey(memberInfo)
                    ? MemberSerializations[memberInfo](memberValue)
                    : PrintToString(memberValue, nestingLevel + 1);

            return valueToPrint;
        }
    }


    public interface IPrintingConfig<TOwner>
    {
        ImmutableDictionary<Type, Func<object, string>> TypesSerialization { get; set; }
        ImmutableDictionary<MemberInfo, Func<object, string>> PropsSerialization { get; set; }
    }
}