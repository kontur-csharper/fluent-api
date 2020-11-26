using System;
using System.Collections.Generic;

namespace ObjectPrinting.Tests
{
    public class Person
    {
        public List<int> Codes;
        public Dictionary<int, int> Passwords;
        public string Surname;
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }
        public Person Friend { get; set; }
        public Person Friend2 { get; set; }
    }
}