using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ObjectPrinting
{
    public class PrintingConfig
    {
        public readonly Dictionary<Type, CultureInfo> CultureTypes =
            new Dictionary<Type, CultureInfo>();

        public readonly HashSet<MemberInfo> ExcludingMembers = new HashSet<MemberInfo>();
        public readonly HashSet<Type> ExcludingTypes = new HashSet<Type>();

        public readonly HashSet<Type> FinalTypes = new HashSet<Type>
        {
            typeof(string), typeof(DateTime), typeof(TimeSpan), typeof(Guid), typeof(DateTimeOffset)
        };

        public readonly Dictionary<MemberInfo, Delegate> PrintingFunctionsForMembers =
            new Dictionary<MemberInfo, Delegate>();

        public readonly Dictionary<Type, Delegate> PrintingFunctionsForTypes =
            new Dictionary<Type, Delegate>();

        public readonly HashSet<object> VisitedObjects = new HashSet<object>();

        public string Indentation = "\t";
        public string SeparatorBetweenNameAndValue = "=";
    }
}