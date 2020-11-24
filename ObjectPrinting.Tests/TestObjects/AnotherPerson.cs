using System;
using System.Collections.Generic;

namespace ObjectPrinting.Tests.TestObjects
{
    public class AnotherPerson : IPerson
    {
        public AnotherPerson()
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