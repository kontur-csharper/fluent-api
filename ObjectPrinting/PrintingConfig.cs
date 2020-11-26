using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FluentAssertions.Types;
using ObjectPrinting.Solved.Tests;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        private readonly ImmutableHashSet<Type> typesToExclude;
        private readonly ImmutableDictionary<Type, Func<object, string>> typesToSerialize;
        private readonly ImmutableHashSet<MemberInfo> membersToExclude;
        private readonly ImmutableDictionary<MemberInfo, Func<object, string>> membersToSerialize;
        private readonly CultureInfo culture;

        public PrintingConfig()
        {
            typesToExclude = ImmutableHashSet<Type>.Empty;
            typesToSerialize = ImmutableDictionary<Type, Func<object, string>>.Empty;
            membersToExclude = ImmutableHashSet<MemberInfo>.Empty;
            membersToSerialize = ImmutableDictionary<MemberInfo, Func<object, string>>.Empty;
        }

        private PrintingConfig(ImmutableHashSet<Type> typesToExclude, 
            ImmutableDictionary<Type, Func<object, string>> typesToSerialize,
            ImmutableHashSet<MemberInfo> membersToExclude,
            ImmutableDictionary<MemberInfo, Func<object, string>> membersToSerialize, CultureInfo culture)
        {
            this.typesToExclude = typesToExclude;
            this.typesToSerialize = typesToSerialize;
            this.culture = culture;
            this.membersToExclude = membersToExclude;
            this.membersToSerialize = membersToSerialize;
        }

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        public PrintingConfig<TOwner> WithoutType<T>()
        {
            return new PrintingConfig<TOwner>(typesToExclude.Add(typeof(T)), typesToSerialize, membersToExclude, membersToSerialize, culture);
        }

        public PrintingConfig<TOwner> SerializeTypeAs<T>(Func<T, string> serializator)
        {
            return new PrintingConfig<TOwner>(typesToExclude, 
                typesToSerialize.Add(typeof(T), obj => serializator((T)obj)), 
                membersToExclude, membersToSerialize, culture);
        }

        public PrintingConfig<TOwner> SetCulture(CultureInfo culture)
        {
            return new PrintingConfig<TOwner>(typesToExclude,
                typesToSerialize,
                membersToExclude, membersToSerialize, culture);
        }

        public PrintingConfig<TOwner> SerializePropertyAs<T>(
            Expression<Func<TOwner, T>> memberSelector, Func<T, string> serializator)
        {
            if (memberSelector.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException();
            var memberAccess = (MemberExpression) memberSelector.Body;
            if (memberAccess.Member.MemberType != MemberTypes.Field &&
                memberAccess.Member.MemberType != MemberTypes.Property)
                throw new ArgumentException();
            return new PrintingConfig<TOwner>(typesToExclude,
                typesToSerialize, membersToExclude,
                membersToSerialize.Add(memberAccess.Member, obj=> serializator((T)obj)),
                culture);
        }

        public PrintingConfig<TOwner> WithoutProperty<T>(Expression<Func<TOwner, T>> memberSelector)
        {
            if (memberSelector.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException();
            var memberAccess = (MemberExpression)memberSelector.Body;
            if (memberAccess.Member.MemberType != MemberTypes.Field &&
                memberAccess.Member.MemberType != MemberTypes.Property)
                throw new ArgumentException();
            return new PrintingConfig<TOwner>(typesToExclude, typesToSerialize,
                membersToExclude.Add(memberAccess.Member),
                membersToSerialize, culture);
        }

        public PrintingConfig<TOwner> TrimStrings(int length)
        {
            return new PrintingConfig<TOwner>(typesToExclude, 
                typesToSerialize.Add(typeof(string), obj => 
                    length < ((string)obj).Length ? 
                        ((string)obj).Substring(0, length) :
                        (string)obj),
                membersToExclude, membersToSerialize, culture);
        }

        private string PrintToString(object obj, int nestingLevel, Dictionary<object, Guid> handledObjects = null)
        {
            if (obj == null)
                return "null" + Environment.NewLine;
            if (handledObjects == null)
                handledObjects = new Dictionary<object, Guid>();
            var finalTypes = new[]
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan), typeof(Guid)
            };
            var type = obj.GetType();
            if (typesToSerialize.ContainsKey(type))
                return typesToSerialize[type](obj) + Environment.NewLine;
            if (finalTypes.Contains(type))
                return (obj is IFormattable formattable && culture != null ?
                    formattable.ToString(null, culture) : obj) + Environment.NewLine;

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            sb.AppendLine($"{type.Name}");
            var members = GetFieldsAndProperties(type);
            var guid = GetGuid(members, obj);
            if (handledObjects.ContainsKey(obj))
            {
                sb.AppendLine($"{identation}Id = {handledObjects[obj]}");
                return sb.ToString();
            }
            handledObjects.Add(obj, guid);
            foreach (var memberInfo in members
                .Where(member => (member is PropertyInfo propertyInfo && !typesToExclude.Contains(propertyInfo.PropertyType)
                    || member is FieldInfo fieldInfo && !typesToExclude.Contains(fieldInfo.FieldType)) 
                                 && !membersToExclude.Contains(member)))
            {
                sb.Append(identation + memberInfo.Name + " = " +
                          (membersToSerialize.ContainsKey(memberInfo) ?
                              membersToSerialize[memberInfo](GetObject(memberInfo, obj)) + Environment.NewLine :
                          PrintToString(GetObject(memberInfo, obj),
                              nestingLevel + 1, handledObjects)));
            }
            return sb.ToString();
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

        private Guid GetGuid(IEnumerable<MemberInfo> members, object obj)
        {
            var guidType = typeof(Guid);
            foreach (var m in members)
            {
                if (m is PropertyInfo property && property.PropertyType == guidType)
                    return (Guid)property.GetValue(obj);
                if (m is FieldInfo field && field.FieldType == guidType)
                    return (Guid)field.GetValue(obj);
            }
            return Guid.NewGuid();
        }
    }
}