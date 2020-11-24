using System;
using System.Collections.Generic;

namespace ObjectPrinting.Tests
{
    public class Person
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public double Height { get; set; }
        
        public int AgeProperty { get; set; }
        
        public Person[] Relatives { get; set; }
        
        public Dictionary<Person, Person> Persons { get; set; }
        
        public readonly int AgeField = 1;
        public int HeightField = 1;

        public string FamilyName = "abc";
    }
}