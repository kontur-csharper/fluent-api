using System.Collections.Generic;

namespace ObjectPrinting.Tests
{
    public class Student
    {
        public string Name { get; set; }
        public List<string> Friends { get; set; }
        public double[] FavoriteRealValues { get; set; }
        public Dictionary<string, int> Marks { get; set; }

        public Student(string name)
        {
            Name = name;
        }
    }
}