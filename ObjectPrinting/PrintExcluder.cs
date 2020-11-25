using System;
using System.Collections.Immutable;
using System.Reflection;

namespace ObjectPrinting
{
    public class PrintExcluder
    {
        private ImmutableHashSet<Type> TypesForExclude { get; }
        private ImmutableHashSet<PropertyInfo> PropertiesForExclude { get; }
        private ImmutableHashSet<FieldInfo> FieldsForExclude { get; }

        public PrintExcluder() : this(
            ImmutableHashSet<Type>.Empty,
            ImmutableHashSet<PropertyInfo>.Empty,
            ImmutableHashSet<FieldInfo>.Empty) 
        { }

        private PrintExcluder(
            ImmutableHashSet<Type> typesForExclude,
            ImmutableHashSet<PropertyInfo> propertiesForExclude,
            ImmutableHashSet<FieldInfo> fieldsForExclude
        )
        {
            TypesForExclude = typesForExclude;
            PropertiesForExclude = propertiesForExclude;
            FieldsForExclude = fieldsForExclude;
        }

        public bool IsExclude(PropertyInfo property) =>
            PropertiesForExclude.Contains(property) || TypesForExclude.Contains(property.PropertyType);

        public bool IsExclude(FieldInfo field) =>
            FieldsForExclude.Contains(field) || TypesForExclude.Contains(field.FieldType);

        public PrintExcluder Exclude(Type type) =>
            new PrintExcluder(TypesForExclude.Add(type), PropertiesForExclude, FieldsForExclude);

        public PrintExcluder Exclude(FieldInfo field) =>
            new PrintExcluder(TypesForExclude, PropertiesForExclude, FieldsForExclude.Add(field));

        public PrintExcluder Exclude(PropertyInfo property) =>
            new PrintExcluder(TypesForExclude, PropertiesForExclude.Add(property), FieldsForExclude);
    }
}