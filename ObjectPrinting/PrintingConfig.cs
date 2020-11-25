using System;
using System.Linq.Expressions;
using System.Reflection;
using ObjectPrinting.Contexts;
using ObjectPrinting.Contracts;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner> : IPrintingConfig
    {
        private PrintExcluder Excluder { get; }
        private AlternativePrinter AlternativePrinter { get; }
        AlternativePrinter IPrintingConfig.AlternativePrinter => AlternativePrinter;
        PrintExcluder IPrintingConfig.PrintExcluder => Excluder;

        public PrintingConfig() : this(new PrintExcluder(), new AlternativePrinter()) { }

        private PrintingConfig(PrintExcluder excluder, AlternativePrinter alternativePrinter)
        {
            Excluder = excluder;
            AlternativePrinter = alternativePrinter;
        }

        public string PrintToString(TOwner obj)
        {
            var serializer = new Serializer(this);
            return serializer.PrintToString(obj, 0);
        }

        public TypePrintingConfig<TOwner, TPropType> Printing<TPropType>() =>
            new TypePrintingConfig<TOwner, TPropType>(this);

        public IContextPrintingConfig<TOwner, TContext> Printing<TContext>(
            Expression<Func<TOwner, TContext>> memberSelector
        )
        {
            var entity = GetClassMemberEntity(memberSelector);

            if (entity is PropertyInfo)
                return new PropertyPrintingConfig<TOwner, TContext>(this, entity);
            return new FieldPrintingConfig<TOwner, TContext>(this, entity);
        }

        public PrintingConfig<TOwner> Excluding<TContext>(Expression<Func<TOwner, TContext>> memberSelector)
        {
            var entity = GetClassMemberEntity(memberSelector);
            return new PrintingConfig<TOwner>(Excluder.Exclude(entity), AlternativePrinter);
        }

        public PrintingConfig<TOwner> Excluding<TPropType>() =>
            new PrintingConfig<TOwner>(Excluder.Exclude(typeof(TPropType)), AlternativePrinter);

        IPrintingConfig IPrintingConfig.AddAlternativePrintingFor(Type type, Func<object, string> print) =>
            new PrintingConfig<TOwner>(Excluder, AlternativePrinter.AddOrUpdate(type, print));

        IPrintingConfig IPrintingConfig.AddAlternativePrintingFor(PropertyInfo property, Func<object, string> print) =>
            new PrintingConfig<TOwner>(Excluder, AlternativePrinter.AddOrUpdate(property, print));

        IPrintingConfig IPrintingConfig.AddAlternativePrintingFor(FieldInfo field, Func<object, string> print) =>
            new PrintingConfig<TOwner>(Excluder, AlternativePrinter.AddOrUpdate(field, print));


        private static dynamic GetClassMemberEntity<TContext>(Expression<Func<TOwner, TContext>> memberSelector)
        {
            return (memberSelector.Body as MemberExpression)?.Member switch
            {
                PropertyInfo property => property,
                FieldInfo field => field,
                _ => throw new ArgumentException($"{memberSelector} should return field or property {nameof(TOwner)}")
            };
        }
    }
}