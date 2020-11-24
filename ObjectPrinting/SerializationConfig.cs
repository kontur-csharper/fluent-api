using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ObjectPrinting
{
    public class SerializationConfig
    {
        public HashSet<Type> ExcludedTypes { get; } = new HashSet<Type>();
        public HashSet<MemberInfo> ExcludedMembers { get; } = new HashSet<MemberInfo>();
        public Dictionary<Type, Func<object, string>> TypePrintingRules { get; } = new Dictionary<Type, Func<object, string>>();
        public Dictionary<MemberInfo, Func<object, string>> MemberPrintingRules { get; } = new Dictionary<MemberInfo, Func<object, string>>();
        public Dictionary<Type, CultureInfo> TypeCultures { get; } = new Dictionary<Type, CultureInfo>();
        public Dictionary<MemberInfo, CultureInfo> MemberCultures { get; } = new Dictionary<MemberInfo, CultureInfo>();
        public Dictionary<MemberInfo, Func<string, string>> MemberTrimToLength { get; } = new Dictionary<MemberInfo, Func<string, string>>();
        public Func<string, string> GlobalTrimToLength { get; set; } = x => x;
        public int MaxNestingLevel { get; } = 100;

        public readonly Type[] FinalTypes =
        {
            typeof(int), typeof(double), typeof(float), typeof(string),
            typeof(DateTime), typeof(TimeSpan)
        };
    }
}