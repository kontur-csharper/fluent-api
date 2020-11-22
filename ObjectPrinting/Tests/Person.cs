using System;
using System.Collections.Generic;

namespace ObjectPrinting.Tests
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }
        public Person Friend { get; set; }
        public string Address;
        public int PhoneNumber;
        public List<Person> Family;
        public Dictionary<string, int> PhoneBook { get; set; }
    }
}