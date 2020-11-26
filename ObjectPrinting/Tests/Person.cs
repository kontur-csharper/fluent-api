using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ObjectPrinting.Tests
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }
        public double Money { get; set; }
        
        public Person InnerPerson { get; set; }
        
        public string Blabla { get; set; }
        
        public List<int> List { get; set; }

        public string[] Array { get; set; }
        
        public Dictionary<int, int> Dictionary { get; set; }

    }

    public class PersonField
    {
        public string Name;
        public int Age;
        public DateTime DateTime;
        public double Money;
    }
    
}