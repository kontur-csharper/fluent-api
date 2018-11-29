﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    internal interface IPrintingConfig
    {
        void AddPropertySerializationFormat(PropertyInfo property, Delegate format);
        void AddTypeSerializationFormat(Type type, Delegate format);
        void AddPostProduction(PropertyInfo property, Delegate format);
        void AddCultureInfo(Type type, CultureInfo cultureInfo);
    }
}
