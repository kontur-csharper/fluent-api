using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectPrinting.Config
{
    public class PrintingConfig<TOwner> : IPrintingConfig<TOwner>
    {
        private readonly HashSet<Type> excludedTypes = new HashSet<Type>();
        private readonly HashSet<PropertyInfo> excludedProperties = new HashSet<PropertyInfo>();
        private readonly HashSet<FieldInfo> excludedFields = new HashSet<FieldInfo>();
        private readonly Dictionary<Type, Delegate> typeToSerializer = new Dictionary<Type, Delegate>();
        private readonly Dictionary<PropertyInfo, Delegate> propertyToSerializer = new Dictionary<PropertyInfo, Delegate>();
        private readonly Dictionary<FieldInfo, Delegate> fieldToSerializer = new Dictionary<FieldInfo, Delegate>();

        HashSet<Type> IPrintingConfig<TOwner>.ExcludedTypes => excludedTypes;
        HashSet<PropertyInfo> IPrintingConfig<TOwner>.ExcludedProperties => excludedProperties;
        HashSet<FieldInfo> IPrintingConfig<TOwner>.ExcludedFields => excludedFields;
        Dictionary<Type, Delegate> IPrintingConfig<TOwner>.TypeToSerializer => typeToSerializer;
        Dictionary<PropertyInfo, Delegate> IPrintingConfig<TOwner>.PropertyToSerializer => propertyToSerializer;
        Dictionary<FieldInfo, Delegate> IPrintingConfig<TOwner>.FieldToSerializer => fieldToSerializer;

        public IConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new TypePrintingConfig<TOwner, TPropType>(this);
        }

        public IConfig<TOwner, TPropType> Printing<TPropType>(
            Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var propertyInfo = ((MemberExpression)memberSelector.Body).Member as PropertyInfo;
            var fieldInfo = ((MemberExpression)memberSelector.Body).Member as FieldInfo;
            ThrowIfNotValidMember(propertyInfo, fieldInfo);
            return propertyInfo != null
                ? (IConfig<TOwner, TPropType>)new PropertyPrintingConfig<TOwner, TPropType>(this, propertyInfo)
                : new FieldPrintingConfig<TOwner, TPropType>(this, fieldInfo);
        }

        public IPrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var propertyInfo = ((MemberExpression)memberSelector.Body).Member as PropertyInfo;
            var fieldInfo = ((MemberExpression)memberSelector.Body).Member as FieldInfo;
            ThrowIfNotValidMember(propertyInfo, fieldInfo);
            _ = propertyInfo != null
                ? excludedProperties.Add(propertyInfo)
                : excludedFields.Add(fieldInfo);
            return this;
        }

        public IPrintingConfig<TOwner> Excluding<TPropType>()
        {
            excludedTypes.Add(typeof(TPropType));
            return this;
        }

        public string PrintToString(TOwner obj)
        {
            return new ObjectSerializer<TOwner>(this).Serialize(obj, 0);
        }

        private static void ThrowIfNotValidMember(PropertyInfo propertyInfo, FieldInfo fieldInfo)
        {
            if (propertyInfo == null && fieldInfo == null)
                throw new ArgumentException("Member selector should define properties or fields");
        }
    }
}