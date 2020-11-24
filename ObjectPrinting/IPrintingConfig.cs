using System;
using System.Globalization;
using System.Linq.Expressions;

namespace ObjectPrinting
{
    public interface IPrintingConfig<TOwner>
    {
        public IPrintingConfig<TOwner> Exclude(params Type[] types);
        public IPrintingConfig<TOwner> Exclude<TProperty>(Expression<Func<TOwner, TProperty>> property);
        public ITypePrintingConfig<TOwner, TPropType> Printing<TPropType>();

        public IPropertyPrintingConfig<TProperty, TOwner> SelectProperty<TProperty>(
            Expression<Func<TOwner, TProperty>> property);

        public IPrintingConfig<TOwner> SetCultureInfo<T>(CultureInfo cultureInfo) where T : IFormattable;
        public string PrintToString(TOwner obj);
        public void AddSerialization(Type type, Delegate action);
        public void AddSerialization(string name, Delegate action);
    }
}