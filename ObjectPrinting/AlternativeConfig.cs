using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectPrinting
{
    public class AlternativeConfig<TOwner>
    {
        private readonly Dictionary<MemberInfo, Delegate> alternativePropertyRules =
            new Dictionary<MemberInfo, Delegate>();
        private readonly Dictionary<Type, Delegate> alternativeTypeRules = new Dictionary<Type, Delegate>();
        private readonly Dictionary<Type, CultureInfo> cultures = new Dictionary<Type, CultureInfo>();
        
        private MemberInfo parameterToAdd;
        
        public void AddRule<TPropType>(Func<TPropType, string> print)
        {
            if (parameterToAdd == null)
            {
                alternativeTypeRules[typeof(TPropType)] = print;
            }
            else
            {
                alternativePropertyRules[parameterToAdd] = print;
                parameterToAdd = null;
            }
        }

        public void AddCulture<TPropType>(CultureInfo cultureInfo) where TPropType : IFormattable
            => cultures[typeof(TPropType)] = cultureInfo;

        public void AddAlternativeProperty<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            var body = (MemberExpression)memberSelector.Body;
            parameterToAdd = body.Member;
        }

        public string GetAlternativeSerialization(MemberInfo propertyInfo, object value)
        {
            if (value == null) return null;
            var type = propertyInfo.GetMemberType();
            
            if (alternativePropertyRules.TryGetValue(propertyInfo, out var propertyRule))
                return (string) propertyRule.DynamicInvoke(value) + Environment.NewLine;
            
            if (alternativeTypeRules.TryGetValue(type, out var typeRule))
                return (string) typeRule.DynamicInvoke(value) + Environment.NewLine;
            
            if (cultures.TryGetValue(type, out var cultureInfo))
                return ((IFormattable) value).ToString(null, cultureInfo) + Environment.NewLine;

            return null;
        }
    }
}