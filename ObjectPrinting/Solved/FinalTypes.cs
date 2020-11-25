using System;
using System.Collections.Generic;

namespace ObjectPrinting.Solved
{
    internal static class FinalTypes
    {
        private static readonly HashSet<Type> finalTypes = new HashSet<Type>
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan), typeof(Guid)
            };

        internal static bool IsFinalType(Type type) => 
            type == null || type.IsPrimitive || finalTypes.Contains(type);
    }
}
