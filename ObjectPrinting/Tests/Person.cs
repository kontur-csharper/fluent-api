using System;

namespace ObjectPrinting.Tests
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Field;
        public Person Father { get; set; }
        public Person Mother { get; set; }
    }
}