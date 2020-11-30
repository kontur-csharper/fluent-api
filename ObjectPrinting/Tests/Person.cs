using System;

namespace ObjectPrinting.Tests
{
    public class Person
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public double Height { get; set; }
        public int Age;
        public Person Parent { get; set; }
        public Pet PersonPet;
        public Pet AnotherPet;
    }
}