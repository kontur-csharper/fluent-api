using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        private HashSet<Type> excludedType = new HashSet<Type>();
        private HashSet<string> excludedProperties = new HashSet<string>();
        private Dictionary<Type, Delegate> specialSerializeTypes = new Dictionary<Type, Delegate>();
        private Dictionary<string, Delegate> specialSerializeProperties = new Dictionary<string, Delegate>();
        private Dictionary<Type, CultureInfo> specialSerializeCulture = new Dictionary<Type, CultureInfo>();
        private Dictionary<string, Delegate> trimmingProperties = new Dictionary<string, Delegate>();

        private int referenceCount = 0;
        internal Configuration configuration = new Configuration();
        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var propertyName = ((MemberExpression)memberSelector.Body).Member.Name;
            
            if(!configuration.ExcudedProperties.ContainsKey(typeof(TOwner)))
                configuration.ExcudedProperties[typeof(TOwner)] = new HashSet<string>();
            configuration.ExcudedProperties[typeof(TOwner)].Add(propertyName);
            
            return this;
        }
        
        public PrintingConfig<TOwner> Excluding<TPropType>()
        {
            if(!configuration.ExcudedTypes.ContainsKey(typeof(TOwner)))
                configuration.ExcudedTypes[typeof(TOwner)] = new HashSet<Type>();
            
            configuration.ExcudedTypes[typeof(TOwner)].Add(typeof(TPropType));
            

            return this;
        }
        
        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(this);
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var propertyName = ((MemberExpression)memberSelector.Body).Member.Name;

            return new PropertyPrintingConfig<TOwner, TPropType>(this, propertyName);
        }
        
        private string PrintToString(object obj, int nestingLevel)
        {
            if (obj == null)
                return "null" + Environment.NewLine;

            if (obj.GetType() == typeof(Guid))
                return obj.ToString() + Environment.NewLine;

            if (nestingLevel == 50)
                return "Nesting level overhead" + Environment.NewLine;
            
            FillConfiguration(obj);
            
            var finalTypes = new[]
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan)
            };

            if (finalTypes.Contains(obj.GetType()))
                return  HandleFinalType(obj);

            var indention = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            
            if (obj is ICollection collection)
                return HandleCollection(collection, nestingLevel, sb, indention);

            var type = obj.GetType();
            sb.AppendLine(type.Name);
            
            sb.Append(HandlePropertyAndFields(type, obj, indention, nestingLevel));

            return sb.ToString();
        }
        
        private string HandlePropertyAndFields(Type type, object obj, string indention, int nestingLevel)
        {
            var stringBuilder = new StringBuilder();
            var allTypes = GetAllTypes(type);
            var allNames = GetAllName(type);
            var allValues = GetAllValues(type, obj);
          
            for(var i =0; i<allTypes.Count; i++)
            {
                var value = allValues[i];

                if (IsReferenceCycle(obj, allTypes[i],value))
                {
                    stringBuilder.Append($"{indention}reference cycle {obj.GetType()}");
                    continue;
                }
                
                if (IsExcluded(allTypes[i], allNames[i]))
                    continue;
            
                var result = ($"{indention} {allNames[i]} = " +
                              $"{PrintToString(GetFormattedValue(value, allNames[i]), nestingLevel + 1)}");
                if (result != stringBuilder.ToString())
                    stringBuilder.Append(result);
            }

            return stringBuilder.ToString();
        }

        private List<object> GetAllValues(Type type, object obj)
        {
            var allValues = new List<object>();
            
            var propertyValues = type.GetProperties().Select(x => x.GetValue(obj)).ToList();

            var fieldValues = type.GetFields().Select(x => x.GetValue(obj)).ToList();

            allValues.AddRange(fieldValues);
            allValues.AddRange(propertyValues);
            return allValues;
        }
        
        private List<string> GetAllName(Type type)
        {
            var allNames = new List<string>();

            var propertyNames = type.GetProperties().Select(x => x.Name).ToList();
      
            var fieldNames = type.GetFields().Select(x => x.Name).ToList();

            allNames.AddRange(fieldNames);
            allNames.AddRange(propertyNames);

            return allNames;
        }
        
        private List<Type> GetAllTypes(Type type)
        {
            var allTypes = new List<Type>();
            
            var propertyTypes = type.GetProperties().Select(x => x.PropertyType).ToList();
            var fieldTypes = type.GetFields().Select(x => x.FieldType).ToList();

            allTypes.AddRange(fieldTypes);
            allTypes.AddRange(propertyTypes);

            return allTypes;
        }
        
        private string HandleCollection(ICollection collection, int nestingLevel, StringBuilder sb, string indention)
        {
            sb.Append(Environment.NewLine);
            foreach (var c in collection)
                sb.Append($"{indention}{indention}{PrintToString(c, nestingLevel)}");
            return sb.ToString();
        }

        private bool IsReferenceCycle(object obj, Type type, object value)
        {
            if (type == obj.GetType() && referenceCount == 1)
                return true;

            if (type == obj.GetType() &&
                ReferenceEquals(value, obj))
                referenceCount++;
            return false;
        }

        private bool IsExcluded(Type type, string name)
        {
            return excludedType.Contains(type) || 
                   excludedProperties.Contains(name);
        }
               
        private object GetFormattedValue(object value, string name)
        {
            if (trimmingProperties.ContainsKey(name))
                value = (string) trimmingProperties[name].DynamicInvoke(value);
     
            if(specialSerializeProperties.ContainsKey(name)) 
                value = (string)specialSerializeProperties[name].DynamicInvoke(value);
            
            return value ;
        }

        private string HandleFinalType(object obj)
        {
            if (specialSerializeCulture.ContainsKey(obj.GetType()))
            {
                if (obj.GetType() == typeof(DateTime))
                    return ((DateTime) obj).ToString(specialSerializeCulture[obj.GetType()]);
                return string.Format(specialSerializeCulture[obj.GetType()], "{0}", obj) + Environment.NewLine;
            }

            if (specialSerializeTypes.ContainsKey(obj.GetType()))
                return (string)specialSerializeTypes[obj.GetType()].DynamicInvoke(obj) + Environment.NewLine;
            
            if (!excludedType.Contains(obj.GetType()))
                return obj + Environment.NewLine;

            return obj + Environment.NewLine;
        }
        
        private void FillConfiguration(object obj)
        {
            if (configuration.TrimmingProperties.ContainsKey(obj.GetType()))
                trimmingProperties = configuration.TrimmingProperties[obj.GetType()];
            
            if (configuration.SpecialSerializeProperties.ContainsKey(obj.GetType()))
                specialSerializeProperties = configuration.SpecialSerializeProperties[obj.GetType()];
            
            if(configuration.ExcudedTypes.ContainsKey(obj.GetType()))
                excludedType = configuration.ExcudedTypes[obj.GetType()];
            
            if (configuration.ExcudedProperties.ContainsKey(obj.GetType()))
                excludedProperties = configuration.ExcudedProperties[obj.GetType()];
            
            if(configuration.SpecialSerializeTypes.ContainsKey(obj.GetType()))
                specialSerializeTypes =configuration.SpecialSerializeTypes[obj.GetType()];
            
            if(configuration.SpecialSerializeCulture.ContainsKey(obj.GetType()))
                specialSerializeCulture =configuration.SpecialSerializeCulture[obj.GetType()];
        }
    }
}