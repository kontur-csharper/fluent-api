using System;

namespace ObjectPrinting.Tests
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }

        public Person(Guid id, string name, double height, int age)
        {
            Id = id;
            Name = name;
            Height = height;
            Age = age;
        }

        public Person()
        {
        }
    }
}