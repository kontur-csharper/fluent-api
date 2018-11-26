using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        private readonly HashSet<Type> excludeTypes = new HashSet<Type>();
        public readonly HashSet<string> properties = new HashSet<string>();

        public readonly Dictionary<string, Delegate> propertyOperations = new Dictionary<string, Delegate>();
        private readonly Dictionary<Type, Delegate> typeOperations = new Dictionary<Type, Delegate>();

        public void SetTypeOperation(Type type, Delegate operation) => typeOperations[type] = operation;

        public void AddTypeOperation(Type type, Delegate operation) => typeOperations.Add(type, operation);

        public PrintingConfig<TOwner> Exclude<TPropertyType>()
        {
            excludeTypes.Add(typeof(TPropertyType));
            return this;
        }

        public SerializingConfig<TOwner, TPropType> Serialize<TPropType>()
        {
            return new SerializingConfig<TOwner, TPropType>(this);
        }

        public SerializingConfig<TOwner, TProperty> Serialize<TProperty>(Expression<Func<TOwner, TProperty>> propSelector)
        {
            var className = typeof(TOwner).ToString();
            var member = (propSelector.Body as MemberExpression)?.ToString();
            member = member?.Substring(member.IndexOf('.'));
            properties.Add(className + member);

            return new SerializingConfig<TOwner, TProperty>(this);
        }

        public PrintingConfig<TOwner> DefaultSerialize()
        {
            return this;
        }

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        public string PrintToString(object obj, int nestingLevel)
        {
            //TODO apply configurations
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
                var propType = propertyInfo.PropertyType;

                if (excludeTypes.Contains(propType))
                    continue;

                sb.Append(identation + propertyInfo.Name + " = ");

                if (typeOperations.ContainsKey(propType))
                {
                    sb.Append(PrintToString(typeOperations[propType].DynamicInvoke(propertyInfo.GetValue(obj)),
                                  nestingLevel + 1));
                }
                else
                {
                    sb.Append(PrintToString(propertyInfo.GetValue(obj), nestingLevel + 1));
                }
            }

            return sb.ToString();
        }
    }
}