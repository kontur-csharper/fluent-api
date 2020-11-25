using System;
using System.Collections.Generic;

namespace ObjectPrintingTests
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }

        public string[] Wallet { get; set; }
        public Dictionary<string, string> RelativesNames { get; set; }
    }
}