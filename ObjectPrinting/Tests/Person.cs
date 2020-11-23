using System;
using System.Collections.Generic;

namespace ObjectPrinting.Tests
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public int Age { get; set; }
        
        public List<Person> Friends { get; set; } = new List<Person>();
        
        public Dictionary<int, string> SomeDict { get; set; } = new Dictionary<int, string>();
    }
}