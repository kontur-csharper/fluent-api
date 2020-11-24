using System;
using System.Collections.Generic;
using ObjectPrinting.Tests.TestObjects;

namespace ObjectPrinting.TestObjects
{
    public class Person : IPerson
    {
        public Person()
        {
            Parents = new List<IPerson>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }
        public List<IPerson> Parents { get; }
    }
}