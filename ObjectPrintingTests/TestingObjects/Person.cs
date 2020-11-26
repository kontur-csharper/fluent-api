using System;

namespace ObjectPrintingTests.TestingObjects
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public TimeSpan SleptToday { get; set; } = new TimeSpan(0, 1, 33);
        public double Height { get; set; }
        public double Width { get; set; }
    }
}
