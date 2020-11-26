using System;
using System.Collections.Generic;

namespace ObjectPrinting
{
    public class ReferenceComparer : IEqualityComparer<object>
    {
        public bool Equals(object x, object y)
        {
            if (x == null || y == null) return false;
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }
    }
}