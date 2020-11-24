using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {
        public SerializationConfig Config { get; } = new SerializationConfig();
        private MemberInfo currentMember;

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>() =>
            new PropertyPrintingConfig<TOwner, TPropType>(this);

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(
            Expression<Func<TOwner, TPropType>> memberSelector)
        {
            currentMember = ((MemberExpression) memberSelector.Body).Member;
            return new PropertyPrintingConfig<TOwner, TPropType>(this);
        }

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            currentMember = ((MemberExpression) memberSelector.Body).Member;
            Config.ExcludedMembers.Add(currentMember);
            return this;
        }

        public PrintingConfig<TOwner> Excluding<TPropType>()
        {
            Config.ExcludedTypes.Add(typeof(TPropType));
            return this;
        }

        public void SetPrintingRule<TPropType>(Func<TPropType, string> print)
        {
            if (currentMember == null)
                Config.TypePrintingRules[typeof(TPropType)] = x => print((TPropType) x);
            else
                Config.MemberPrintingRules[currentMember] = x => print((TPropType) x);
            currentMember = null;
        }

        public void SetCulture<TPropType>(CultureInfo culture)
        {
            if (currentMember == null)
                Config.TypeCultures[typeof(TPropType)] = culture;
            else
                Config.MemberCultures[currentMember] = culture;
            currentMember = null;
        }

        public void SetMaxLength(int maxLen)
        {
            if (currentMember == null)
                Config.GlobalTrimToLength = x => x.Substring(0, Math.Min(x.Length, maxLen));
            else
                Config.MemberTrimToLength[currentMember] = x => x.Substring(0, Math.Min(x.Length, maxLen));
            currentMember = null;
        }

        public string PrintToString(TOwner obj)
            => new Serializer(Config).Serialize(obj);
    }
}