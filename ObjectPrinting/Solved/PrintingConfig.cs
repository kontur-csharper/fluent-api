using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ObjectPrinting.Solved
{
    public class PrintingConfig<TOwner>
    {
        private static readonly Type[] finalTypes = 
        {
            typeof(int), typeof(double), typeof(float), typeof(string),
            typeof(DateTime), typeof(TimeSpan), typeof(Guid)
        };
        public readonly PrintingParameters Parameters;

        public PrintingConfig()
        {
            Parameters = PrintingParameters.Default;
        }

        internal PrintingConfig(PrintingParameters parameters)
        {
            Parameters = parameters;
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new PropertyPrintingConfig<TOwner, TPropType>
                (this, typeof(TPropType));
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {

            if (memberSelector.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException();
            var memberAccess = (MemberExpression)memberSelector.Body;
            if (memberAccess.Member.MemberType != MemberTypes.Field &&
                memberAccess.Member.MemberType != MemberTypes.Property)
                throw new ArgumentException();
            return new PropertyPrintingConfig<TOwner, TPropType>
                (this, memberAccess.Member);
        }

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            if (memberSelector.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException();
            var memberAccess = (MemberExpression)memberSelector.Body;
            if (memberAccess.Member.MemberType != MemberTypes.Field &&
                memberAccess.Member.MemberType != MemberTypes.Property)
                throw new ArgumentException();
            return new PrintingConfig<TOwner>(Parameters.AddMemberToExclude(memberAccess.Member));
        }

        internal PrintingConfig<TOwner> Excluding<TPropType>()
        {
            return new PrintingConfig<TOwner>(Parameters.AddTypeToExclude(typeof(TPropType)));
        }

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        private string PrintToString(object obj, int nestingLevel, int id = 0, Dictionary<object, int> handledObjects = null)
        {
            if (obj == null)
                return "null" + Environment.NewLine;
            var type = obj.GetType();
            if (Parameters.typesToSerialize.ContainsKey(type) && !Parameters.typesToExclude.Contains(type))
                return Parameters.typesToSerialize[type](obj) + Environment.NewLine;
            if (finalTypes.Contains(type))
                return obj + Environment.NewLine;
            if (handledObjects == null)
                handledObjects = new Dictionary<object, int>();
            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            if (handledObjects.ContainsKey(obj))
            {
                sb.AppendLine($"{type.Name} Id = {handledObjects[obj]}");
                return sb.ToString();
            }
            sb.AppendLine($"{type.Name} Id = {id}");
            handledObjects.Add(obj, id);
            if (obj is IEnumerable) SerializeEnumerable(obj, nestingLevel, id, handledObjects, sb, identation);
            else SerializeProperties(obj, nestingLevel, id, handledObjects, type, sb, identation);
            return sb.ToString();
        }

        private void SerializeEnumerable(object obj, int nestingLevel, int id, Dictionary<object, int> handledObjects, StringBuilder sb,
            string identation)
        {
            foreach (var element in (IEnumerable) obj)
            {
                sb.Append(identation + PrintToString(element,
                    nestingLevel + 1, id + 1, handledObjects));
            }
        }

        private void SerializeProperties(object obj, int nestingLevel, int id, Dictionary<object, int> handledObjects, Type type,
            StringBuilder sb, string identation)
        {
            foreach (var memberInfo in GetFieldsAndProperties(type)
                .Where(member =>
                    (member is PropertyInfo propertyInfo && !Parameters.typesToExclude.Contains(propertyInfo.PropertyType)
                     || member is FieldInfo fieldInfo && !Parameters.typesToExclude.Contains(fieldInfo.FieldType))
                    && !Parameters.membersToExclude.Contains(member)))
            {
                sb.Append(identation + memberInfo.Name + " = " +
                          (Parameters.membersToSerialize.ContainsKey(memberInfo)
                              ? Parameters.membersToSerialize[memberInfo](GetObject(memberInfo, obj)) + Environment.NewLine
                              : PrintToString(GetObject(memberInfo, obj),
                                  nestingLevel + 1, id + 1, handledObjects)));
            }
        }

        private object GetObject(MemberInfo member, object obj)
        {
            if (member is PropertyInfo propertyInfo)
                return propertyInfo.GetValue(obj);
            if (member is FieldInfo fieldInfo)
                return fieldInfo.GetValue(obj);
            throw new ArgumentException("Wrong member type");
        }

        private IEnumerable<MemberInfo> GetFieldsAndProperties(Type type)
        {
            foreach (var property in type.GetProperties())
                yield return property;
            foreach (var fields in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                yield return fields;
        }
    }
}