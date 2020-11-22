using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner> : IPrintingConfig<TOwner>
    {
        private readonly HashSet<Type> excludedTypes = new HashSet<Type>();
        private readonly Dictionary<Type, Delegate> typeSerialization = new Dictionary<Type, Delegate>();
        private readonly Dictionary<PropertyInfo, Delegate> propertySerialization = new Dictionary<PropertyInfo, Delegate>();
        private readonly Dictionary<FieldInfo, Delegate> fieldSerialization = new Dictionary<FieldInfo, Delegate>();
        private readonly HashSet<PropertyInfo> excludedProperties = new HashSet<PropertyInfo>();
        private readonly HashSet<FieldInfo> excludedFields = new HashSet<FieldInfo>();

        #region IPrintingConfig Property Init
        HashSet<Type> IPrintingConfig<TOwner>.ExcludedTypes => excludedTypes;
        Dictionary<Type, Delegate> IPrintingConfig<TOwner>.TypeSerialization => typeSerialization;
        Dictionary<PropertyInfo, Delegate> IPrintingConfig<TOwner>.PropertySerialization => propertySerialization;
        HashSet<PropertyInfo> IPrintingConfig<TOwner>.ExcludedProperties => excludedProperties;
        HashSet<FieldInfo> IPrintingConfig<TOwner>.ExcludedFields => excludedFields;
        Dictionary<FieldInfo, Delegate> IPrintingConfig<TOwner>.FieldSerialization => fieldSerialization;
        #endregion

        public string PrintToString(TOwner obj)
        {
            var serializer = new ObjectSerializer<TOwner>(this);
            return serializer.Serialize(obj, 0);
        }

        public IConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new TypePrintingConfig<TOwner, TPropType>(this);
        }

        public IConfig<TOwner, TPropType> Printing<TPropType>(
            Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var propertyInfo = ((MemberExpression)memberSelector.Body).Member as PropertyInfo;
            var fieldInfo = ((MemberExpression)memberSelector.Body).Member as FieldInfo;
            MemberSelectorValidator(propertyInfo, fieldInfo);
            return propertyInfo != null ?
               (IConfig<TOwner, TPropType>)new PropertyPrintingConfig<TOwner, TPropType>(this, propertyInfo) :
                new FieldPrintingConfig<TOwner, TPropType>(this, fieldInfo);
        }

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var propertyInfo = ((MemberExpression)memberSelector.Body).Member as PropertyInfo;
            var fieldInfo = ((MemberExpression)memberSelector.Body).Member as FieldInfo;
            MemberSelectorValidator(propertyInfo, fieldInfo);
            _ = propertyInfo != null ? excludedProperties.Add(propertyInfo) : excludedFields.Add(fieldInfo);
            return this;
        }

        public PrintingConfig<TOwner> Excluding<TPropType>()
        {
            excludedTypes.Add(typeof(TPropType));
            return this;
        }

        private void MemberSelectorValidator(PropertyInfo propertyInfo, FieldInfo fieldInfo)
        {
            if (propertyInfo == null && fieldInfo == null)
                throw new ArgumentException("Member selector should define properties or fields");
        }
    }
}