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

        public string Nickname;
        public List<Person> Parents;
        public Person Friend;
        public Dictionary<string, double> Wallet;
        public double FortuneInDollars;
    }
}