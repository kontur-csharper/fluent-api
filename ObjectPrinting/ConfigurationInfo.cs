using System;
using System.Collections.Immutable;

namespace ObjectPrinting
{
    internal class ConfigurationInfo
    {
        private ImmutableHashSet<Type> typesToExclude;
        private ImmutableHashSet<string> membersToExclude;
        private ImmutableDictionary<Type, Delegate> usingForTypes;
        private ImmutableDictionary<string, Delegate> usingForMembers;

        internal ConfigurationInfo()
        {
            typesToExclude = ImmutableHashSet<Type>.Empty;
            membersToExclude = ImmutableHashSet<string>.Empty;
            usingForTypes = ImmutableDictionary<Type, Delegate>.Empty;
            usingForMembers = ImmutableDictionary<string, Delegate>.Empty;
        }

        private ConfigurationInfo(ConfigurationInfo configurationInfo)
        {
            typesToExclude = configurationInfo.typesToExclude;
            membersToExclude = configurationInfo.membersToExclude;
            usingForTypes = configurationInfo.usingForTypes;
            usingForMembers = configurationInfo.usingForMembers;
        }

        internal bool ShouldExclude(Type memberType, string memberName) =>
            typesToExclude.Contains(memberType) || membersToExclude.Contains(memberName);

        internal ConfigurationInfo AddPropertyToExclude(string memberName) =>
            new ConfigurationInfo(this) {membersToExclude = membersToExclude.Add(memberName)};

        internal ConfigurationInfo AddTypeToExclude(Type memberType) =>
            new ConfigurationInfo(this) {typesToExclude = typesToExclude.Add(memberType)};

        internal ConfigurationInfo AddUsingForType<TPropType>(Func<TPropType, string> print)
        {
            var updatedUsingForTypes = usingForTypes.ContainsKey(typeof(TPropType))
                ? usingForTypes.SetItem(typeof(TPropType), print)
                : usingForTypes.Add(typeof(TPropType), print);
            return new ConfigurationInfo(this) {usingForTypes = updatedUsingForTypes};
        }

        internal ConfigurationInfo AddUsingForProperty<TPropType>(Func<TPropType, string> print, string propertyName)
        {
            var updatedUsingForProperties = usingForMembers.ContainsKey(propertyName)
                ? usingForMembers.SetItem(propertyName, print)
                : usingForMembers.Add(propertyName, print);
            return new ConfigurationInfo(this) {usingForMembers = updatedUsingForProperties};
        }

        internal string TryUseConfiguration(ValueInfo valueInfo, string nestedNames)
        {
            usingForMembers.TryGetValue(nestedNames, out var configureAction);
            if (configureAction == null)
                usingForTypes.TryGetValue(valueInfo.Type, out configureAction);
            return (string) configureAction?.DynamicInvoke(valueInfo.Value);
        }
    }
}