using System.Collections.Generic;
using System.Linq;

namespace ObjectPrinting.Tests
{
    public class Person
    {
        public string Name { get; set; }
        public Car Car { get; set; }
        public List<Person> Children { get; set; }
        public Person Parent { get; set; }
        public Person[] ChildrenArray => Children.Count > 0 ? Children.ToArray() : null;
        public Dictionary<string, Person> ChildrenDict => Children.ToDictionary(c => c.Name, c => c);

        public Person(string name, Car car, Person parent)
        {
            Name = name;
            Children = new List<Person>();
            Parent = parent;
            Car = car;
        }

        public void AddChild(string childName)
        {
            Children.Add(new Person(childName, null, this));
        }
    }
}