using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        private static readonly Type[] finalTypes = new[]
        {
            typeof(int), typeof(double), typeof(float), typeof(string),
            typeof(DateTime), typeof(TimeSpan)
        };
        
        private ConfigInfo configInfo;

        public PrintingConfig()
        {
            configInfo = new ConfigInfo();
        }
        
        public PrintingConfig(ConfigInfo configInfo)
        {
            this.configInfo = configInfo;
        }
        
        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, new HashSet<object>());
        }

        public PrintingConfig<TOwner> Exclude<TProperty>()
        {
            return new PrintingConfig<TOwner>(configInfo.AddToExcluding(typeof(TProperty)));
        }
        
        public PrintingConfig<TOwner> Exclude<TProperty>(
            Expression<Func<TOwner, TProperty>> selectorExpression)
        {
            var propertyToExclude = (PropertyInfo)((MemberExpression) selectorExpression.Body).Member;
            return new PrintingConfig<TOwner>(configInfo.AddToExcluding(propertyToExclude));
        }

        public PropertyPrintingConfig<TOwner, TProperty> Printing<TProperty>()
        {
            return new PropertyPrintingConfig<TOwner, TProperty>(this);
        }

        public PropertyPrintingConfig<TOwner, TProperty> Printing<TProperty>(
            Expression<Func<TOwner, TProperty>> selectorExpression)
        {
            var propertyToExclude = (PropertyInfo)((MemberExpression) selectorExpression.Body).Member;
            return new PropertyPrintingConfig<TOwner, TProperty>(this, propertyToExclude);
        }
        
        // Я сделал данный метод internal для того, чтобы его не было видно вне проекта.
        // Для того, чтобы его использовать, необходимо сперва вызвать Printing, затем Using (или что-то другое)
        // Разработчику, который будет использовать этот класс в дальнейшем, не стоит показывать лишние методы
        internal PrintingConfig<TOwner> SetCustomSerialization<TProperty>(Func<TProperty, string> serialization,
            PropertyInfo propertyToCustomize)
        {
            if (propertyToCustomize != null)
                return new PrintingConfig<TOwner>(configInfo
                    .SetCustomSerialization(serialization, propertyToCustomize));
            return new PrintingConfig<TOwner>(configInfo
                .SetCustomSerialization(serialization));
        }
        
        private string PrintToString(object obj, HashSet<object> parents, string newLine = null)
        {
            newLine ??= Environment.NewLine;
            if (obj == null)
                return "null" + newLine;
            
            if (parents.Contains(obj))
                return $"Cycling reference. Type: {obj.GetType().Name}" + newLine;

            if (finalTypes.Contains(obj.GetType()))
                return obj + newLine;

            parents.Add(obj);

            var identation = new string('\t', parents.Count);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
            {
                if (configInfo.ShouldToExclude(propertyInfo))
                    continue;
                
                sb.Append(identation + propertyInfo.Name + " = ");
                
                sb.Append(Serialize(obj, propertyInfo, parents));
            }
            parents.Remove(obj);
            return sb.ToString();
        }

        private string Serialize(object obj, PropertyInfo propertyInfo, HashSet<object> parents)
        {
            var propertyValue = propertyInfo.GetValue(obj);
            
            if (propertyValue is ICollection collectionProperty)
            {
                return collectionProperty is IDictionary dictionaryProperty 
                    ? SerializeDictionary(dictionaryProperty, parents) 
                    : SerializeCollection(collectionProperty, parents);
            }

            return SerializeProperty(propertyValue, propertyInfo, parents);
        }
        
        // Не нравится что метод похож на SerializeCollection, но как их объединить я не знаю
        private string SerializeDictionary(IDictionary collection, HashSet<object> parents)
        {
            if (collection.Count == 0)
                return "Empty dictionary" + Environment.NewLine;
            
            var sb = new StringBuilder();
            sb.Append(Environment.NewLine + new string('\t', parents.Count) + '{' + Environment.NewLine);
            
            foreach (var key in collection.Keys)
            {
                sb.Append(new string('\t', parents.Count + 1));
                sb.Append(PrintToString(key, parents, ""));
                sb.Append(": ");
                sb.Append(PrintToString(collection[key], parents));
            }

            sb.Append(new string('\t', parents.Count) + '}');

            sb.Append(Environment.NewLine);
            return sb.ToString();
        }
        
        private string SerializeCollection(ICollection collection, HashSet<object> parents)
        {
            if (collection.Count == 0)
                return "Empty collection" + Environment.NewLine;
            
            var sb = new StringBuilder();
            sb.Append(Environment.NewLine + new string('\t', parents.Count) + '[' + Environment.NewLine);
            
            foreach (var e in collection)
            {
                sb.Append(new string('\t', parents.Count));
                sb.Append(PrintToString(e, parents));
            }

            sb.Append(new string('\t', parents.Count) + ']');

            sb.Append(Environment.NewLine);
            return sb.ToString();
        }
        
        private string SerializeProperty(object propertyValue, PropertyInfo propertyInfo, HashSet<object> parents)
        {
            var serialization = configInfo.GetSpecialSerialization(propertyInfo);
                
            if (propertyValue == null)
                return "null" + Environment.NewLine;
            if (serialization != null)
                return serialization.DynamicInvoke(propertyValue) + Environment.NewLine;
            return PrintToString(propertyValue, parents);
        }
    }
}