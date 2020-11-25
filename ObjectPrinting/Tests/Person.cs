using System;
using System.Collections.Generic;

namespace ObjectPrinting.Tests
{
    public class Person
    {
        public Dictionary<string, Person> Family { get; set; }
        public Person Son { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }

        public List<string> FamilyNames { get; set; }
    }
}