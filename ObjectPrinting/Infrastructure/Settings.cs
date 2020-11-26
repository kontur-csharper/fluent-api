using System;
using System.Globalization;

namespace ObjectPrinting.Infrastructure
{
    public class Settings
    {
        public bool IsExcluded;
        public Delegate Printer;
        public int? MaxLength;
        public CultureInfo CultureInfo;
    }
}