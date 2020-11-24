using System;
using System.Collections.Generic;

namespace ObjectPrinting.Tests
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }
        public int Weight;
        public List<Person> Children { get; }
        public Dictionary<string, int> CustomDict { get; }
        public Person Parent { get; set; }

        public Person()
        {
            Children = new List<Person>();
            CustomDict = new Dictionary<string, int>();
        }
    }

    public class Foo
    {
        public string Name { get; set; }
        public Foo Next { get; set; }
        public Foo Friend { get; set; }
    }

    public class Bar
    {
        public Foo Foo { get; set; }
        public string Name { get; set; }
    }

    class FooWithEquals
    {
        public string Name { get; set; }
        public FooWithEquals Next { get; set; }

        protected bool Equals(FooWithEquals other) => Name == other.Name;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FooWithEquals) obj);
        }

        public override int GetHashCode() => (Name != null ? Name.GetHashCode() : 0);
    }
}