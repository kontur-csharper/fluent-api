namespace ObjectPrinting.Tests
{
    public class Animal
    {
        public Animal(string name, string kind, Animal parent = null)
        {
            Name = name;
            Kind = kind;
            Parent = parent;
        }

        public string Name { get; set; }
        public Animal Parent { get; set; }
        public string Kind { get; set; }
        
    }
}