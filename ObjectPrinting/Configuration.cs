using System;
using System.Collections.Generic;
using System.Globalization;

namespace ObjectPrinting
{
    public class Configuration
    {
        public Dictionary<Type, Dictionary<Type, CultureInfo>> SpecialSerializeCulture;
        
        public Dictionary<Type, HashSet<Type>> ExcudedTypes;
        public Dictionary<Type, HashSet<string>> ExcudedProperties;

        public Dictionary<Type, Dictionary<Type, Delegate>> SpecialSerializeTypes;
        public Dictionary<Type, Dictionary<string, Delegate>> SpecialSerializeProperties;

        public Dictionary<Type, Dictionary<string, Delegate>>TrimmingProperties;

        public Configuration()
        {
            SpecialSerializeCulture = new Dictionary<Type, Dictionary<Type, CultureInfo>>();
            
            ExcudedTypes = new Dictionary<Type, HashSet<Type>>();
            ExcudedProperties = new Dictionary<Type, HashSet<string>>();
            
            SpecialSerializeTypes =new Dictionary<Type, Dictionary<Type, Delegate>>();

            
            SpecialSerializeProperties = new Dictionary<Type, Dictionary<string, Delegate>>();
            
            TrimmingProperties = new Dictionary<Type, Dictionary<string, Delegate>>();
        }
    }
}