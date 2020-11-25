using System;
using System.Collections.Generic;

namespace ObjectPrinting.Solved
{
    internal class AlternativeSerializator
    {
        private readonly Dictionary<Type, Delegate> alternativeSerialization = new Dictionary<Type, Delegate>();
        private readonly Dictionary<string, Delegate> alternativeSerializationField = new Dictionary<string, Delegate>();

        private void AddSerialization<TPropType>(Func<TPropType, string> func) =>
            alternativeSerialization[typeof(TPropType)] = func;

        internal void AddSerialization<TPropType>(Func<TPropType, string> func, string fullName = null)
        {
            if (fullName == null)
                AddSerialization(func);
            else
                alternativeSerializationField[fullName] = func;
        }

        internal bool TrySerializate(object obj, Type type, string fullName, out string result) 
        {
            if(fullName != null && alternativeSerializationField.TryGetValue(fullName, out var func) ||
               (type != null && alternativeSerialization.TryGetValue(type, out func)))
            {
                result = func.DynamicInvoke(obj).ToString();
                return true;
            }
            result = null;
            return false;
        }
    }
}
