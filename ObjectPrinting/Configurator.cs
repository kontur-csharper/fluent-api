using System;
using System.Globalization;
using System.Linq.Expressions;

namespace ObjectPrinting
{
    public class Configurator<TOwner>
    {
        private readonly PrintingConfig printingConfig = new PrintingConfig();

        public Configurator<TOwner> AddCultureForType<TPropType>(CultureInfo culture)
        {
            if (typeof(IFormattable).IsAssignableFrom(typeof(TPropType)))
                printingConfig.CultureTypes[typeof(TPropType)] = culture;
            return this;
        }

        public IPropertyConfigurator<TOwner, TPropType> Printing<TPropType>()
        {
            return new PropertyConfigurator<TOwner, TPropType>(this,
                func => printingConfig.PrintingFunctionsForTypes[typeof(TPropType)] = func);
        }

        public IPropertyConfigurator<TOwner, TPropType> Printing<TPropType>(
            Expression<Func<TOwner, TPropType>> memberSelector)
        {
            if (!(memberSelector.Body is MemberExpression))
                throw new InvalidCastException("unable to cast to MemberExpression");
            var member = ((MemberExpression) memberSelector.Body).Member;
            return new PropertyConfigurator<TOwner, TPropType>(this,
                func => printingConfig.PrintingFunctionsForMembers[member] = func);
        }

        public Configurator<TOwner> Excluding<TPropType>(Expression<Func<TOwner, TPropType>> memberSelector)
        {
            if (!(memberSelector.Body is MemberExpression))
                throw new InvalidCastException("unable to cast to MemberExpression");
            printingConfig.ExcludingMembers.Add(((MemberExpression) memberSelector.Body).Member);
            return this;
        }

        public Configurator<TOwner> Excluding<TPropType>()
        {
            printingConfig.ExcludingTypes.Add(typeof(TPropType));
            return this;
        }

        public Configurator<TOwner> ChangeIndentation(string newIndentation)
        {
            printingConfig.Indentation = newIndentation;
            return this;
        }

        public Configurator<TOwner> ChangeSeparatorBetweenNameAndValue(string separator)
        {
            printingConfig.SeparatorBetweenNameAndValue = separator;
            return this;
        }

        public PrintingConfig Build()
        {
            return printingConfig;
        }
    }
}