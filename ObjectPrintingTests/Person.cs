using System;

namespace ObjectPrintingTests
{
    public class Person
    {
        public string Surname;
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }
    }
}