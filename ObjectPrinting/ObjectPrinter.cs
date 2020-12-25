using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public class ObjectPrinter<TOwner>
    {
        private IPrintingConfig config;
        private Stack<object> visitedObjects;
        private HashSet<Type> finalTypes = new HashSet<Type>()
        {
            typeof(int), typeof(double), typeof(float), typeof(string),
            typeof(DateTime), typeof(TimeSpan)
        };

        public ObjectPrinter(PrintingConfig<TOwner> config)
        {
            this.config = config;
            visitedObjects = new Stack<object>();
        }

        public static PrintingConfig<TOwner> Should()
        {
            return new PrintingConfig<TOwner>();
        }

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        private string PrintToString(object obj, int nestingLevel)
        {
            if (obj == null)
                return "null";
            var objType = obj.GetType();
            if (visitedObjects.Contains(obj))
                return $"{obj.GetType().Name} (Looped)";
            visitedObjects.Push(obj);
            if (finalTypes.Contains(objType))
                return PrintFinalType(obj);
            if (typeof(IEnumerable).IsAssignableFrom(objType))
                return PrintEnumerable(obj, nestingLevel);
            return PrintObject(obj, nestingLevel);
        }

        private string PrintObject(object obj, int nestingLevel)
        {
            var resultString = new StringBuilder().Append($"{obj.GetType().Name}{Environment.NewLine}");
            resultString.Append(PrintFields(obj, nestingLevel));
            resultString.Append(PrintProperties(obj, nestingLevel));
            visitedObjects.Pop();
            return resultString.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }

        private string PrintProperties(object obj, int nestingLevel)
        {
            var resultString = new StringBuilder();
            var properties = obj.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (!config.State.ExcludedMembers.Contains(property) 
                    && !config.State.ExcludedTypes.Contains(property.PropertyType))
                    resultString.Append(PrintProperty(property, obj, nestingLevel)).Append(Environment.NewLine);
            }

            return resultString.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }

        private string PrintProperty(PropertyInfo property, object obj, int nestingLevel)
        {
            var propertyString = new StringBuilder().Append(GetIndentation(nestingLevel))
                .Append($"{property.Name} = ");
            if (TryGetAltSerializerFor(property, out var serializer))
                propertyString.Append((string)serializer.DynamicInvoke(property.GetValue(obj)));
            else
                propertyString.Append(PrintToString(property.GetValue(obj), nestingLevel + 1));
            return propertyString.ToString();
        }

        private string PrintFields(object obj, int nestingLevel)
        {
            var resultString = new StringBuilder();
            var fields = obj.GetType().GetFields();
            foreach (var field in fields)
            {
                if (!config.State.ExcludedMembers.Contains(field) 
                    && !config.State.ExcludedTypes.Contains(field.FieldType))
                    resultString.Append(PrintField(field, obj, nestingLevel)).Append(Environment.NewLine);
            }

            return resultString.ToString();
        }

        private string PrintField(FieldInfo field, object obj, int nestingLevel)
        {
            var propertyString = new StringBuilder().Append(GetIndentation(nestingLevel))
                .Append($"{field.Name} = ");
            if (TryGetAltSerializerFor(field, out var serializer))
                propertyString.Append((string)serializer.DynamicInvoke(field.GetValue(obj)));
            else
                propertyString.Append(PrintToString(field.GetValue(obj), nestingLevel + 1));
            return propertyString.ToString();
        }

        private bool TryGetAltSerializerFor(MemberInfo member, out Delegate serializer)
        {
            if (config.State.AltSerializerForMember.TryGetValue(member, out serializer))
                return true;
            var memberType = member is PropertyInfo prop
                ? prop.PropertyType
                : ((FieldInfo) member).FieldType;
            if (TryGetAltSerializerFor(memberType, out serializer))
                return true;

            serializer = null;
            return false;
        }

        private bool TryGetAltSerializerFor(Type type, out Delegate serializer)
        {
            if (config.State.AltSerializerForType.ContainsKey(type))
            {
                serializer = config.State.AltSerializerForType[type];
                return true;
            }

            serializer = null;
            return false;
        }

        private string PrintEnumerable(object obj, int nestingLevel)
        {
            var resultString = new StringBuilder().Append(obj.GetType().Name).Append(Environment.NewLine);
            var indentation = GetIndentation(nestingLevel);
            foreach (var subObj in (IEnumerable)obj)
            {
                resultString.Append(indentation);
                resultString.Append(PrintToString(subObj, nestingLevel + 1)).Append(Environment.NewLine);
            }
            
            visitedObjects.Pop();
            return resultString.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }

        private string PrintFinalType(object obj)
        {
            var resultString = new StringBuilder();
            resultString.Append(config.State.CultureForType.TryGetValue(obj.GetType(), out var culture)
                ? ((IFormattable)obj).ToString("N", culture)
                : obj.ToString());
            visitedObjects.Pop();
            return resultString.ToString();
        }

        private string GetIndentation(int nestingLevel)
        {
            return string.Concat(Enumerable.Repeat("\t", nestingLevel + 1));
        }
    }
}