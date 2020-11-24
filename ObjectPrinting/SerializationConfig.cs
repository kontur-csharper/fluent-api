using System;
using System.Globalization;
using System.Linq.Expressions;

namespace ObjectPrinting
{
    public class SerializationConfig<TOwner>
    {
        protected PrintingConfig<TOwner> ParentConfig;
        protected readonly ExcludingConfig<TOwner> ExcludingConfig = new ExcludingConfig<TOwner>();
        protected readonly AlternativeConfig<TOwner> AlternativeConfig = new AlternativeConfig<TOwner>();

        public PrintingConfig<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            ExcludingConfig.Excluding(memberSelector);
            return ParentConfig;
        }

        internal PrintingConfig<TOwner> Excluding<TPropType>()
        {
            ExcludingConfig.Excluding<TPropType>();
            return ParentConfig;
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>() =>
            new PropertyPrintingConfig<TOwner, TPropType>(ParentConfig);

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(
            Expression<Func<TOwner, TPropType>> memberSelector)
        {
            AlternativeConfig.AddAlternativeProperty(memberSelector);
            return new PropertyPrintingConfig<TOwner, TPropType>(ParentConfig);
        }
        
        public void AddRule<TPropType>(Func<TPropType, string> print)
        {
            AlternativeConfig.AddRule(print);
        }

        public void AddCulture<TPropType>(CultureInfo cultureInfo) where TPropType : IFormattable
            => AlternativeConfig.AddCulture<TPropType>(cultureInfo);
    }
}