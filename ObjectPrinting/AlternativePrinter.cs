using System;
using System.Collections.Immutable;
using System.Reflection;

namespace ObjectPrinting
{
    public class AlternativePrinter
    {
        private ImmutableDictionary<Type, Func<object, string>> PrintsForTypes { get; }
        private ImmutableDictionary<PropertyInfo, Func<object, string>> PrintsForProps { get; }
        private ImmutableDictionary<FieldInfo, Func<object, string>> PrintsForFields { get; }

        public AlternativePrinter() : this(
            ImmutableDictionary<Type, Func<object, string>>.Empty,
            ImmutableDictionary<PropertyInfo, Func<object, string>>.Empty,
            ImmutableDictionary<FieldInfo, Func<object, string>>.Empty) 
        { }

        private AlternativePrinter(
            ImmutableDictionary<Type, Func<object, string>> printsForTypes,
            ImmutableDictionary<PropertyInfo, Func<object, string>> printsForProps,
            ImmutableDictionary<FieldInfo, Func<object, string>> printsForFields
        )
        {
            PrintsForTypes = printsForTypes;
            PrintsForProps = printsForProps;
            PrintsForFields = printsForFields;
        }

        public bool HasAlternativePrintFor(object value) =>
            value != default && PrintsForTypes.ContainsKey(value.GetType());

        public bool HasAlternativePrintFor(PropertyInfo property) =>
            property != default && PrintsForProps.ContainsKey(property);

        public bool HasAlternativePrintFor(FieldInfo field) =>
            field != default && PrintsForFields.ContainsKey(field);

        public AlternativePrinter AddOrUpdate(Type type, Func<object, string> print) =>
            new AlternativePrinter
            (
                AddOrUpdateDictionary(PrintsForTypes, type, print),
                PrintsForProps,
                PrintsForFields
            );

        public AlternativePrinter AddOrUpdate(PropertyInfo property, Func<object, string> print) =>
            new AlternativePrinter
            (
                PrintsForTypes,
                AddOrUpdateDictionary(PrintsForProps, property, print),
                PrintsForFields
            );

        public AlternativePrinter AddOrUpdate(FieldInfo field, Func<object, string> print) =>
            new AlternativePrinter
            (
                PrintsForTypes,
                PrintsForProps,
                AddOrUpdateDictionary(PrintsForFields, field, print)
            );
        
        public string SerializeValue(object obj) => PrintsForTypes[obj.GetType()](obj);
        public string SerializeProperty(PropertyInfo property, object obj) => PrintsForProps[property](obj);
        public string SerializeField(FieldInfo field, object obj) => PrintsForFields[field](obj);

        private static ImmutableDictionary<TKey, Func<object, string>> AddOrUpdateDictionary<TKey>(
            ImmutableDictionary<TKey, Func<object, string>> dict, 
            TKey entity,
            Func<object, string> print
        )
        {
            if (!dict.ContainsKey(entity)) 
                return dict.Add(entity, print);
            var previousFunc = dict[entity];
            return dict.SetItem(entity, obj => print(previousFunc(obj)));
        }
    }
}