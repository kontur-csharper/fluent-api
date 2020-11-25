using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ObjectPrinting
{
    public class Config
    {
        public Dictionary<PropertyInfo, Delegate> PropPrintingMethods =
            new Dictionary<PropertyInfo, Delegate>();

        public HashSet<PropertyInfo> PropsToExclude = new HashSet<PropertyInfo>();
        public Dictionary<Type, CultureInfo> TypePrintingCultureInfo = new Dictionary<Type, CultureInfo>();
        public Dictionary<Type, Delegate> TypePrintingMethods = new Dictionary<Type, Delegate>();
        public HashSet<Type> TypesToExclude = new HashSet<Type>();
    }
}