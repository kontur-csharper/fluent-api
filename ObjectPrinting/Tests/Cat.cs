namespace ObjectPrinting.Tests
{
    public class Cat
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public int WhiskersCount { get; set; }
        public readonly string SomeField = "SomeValue";

        public Cat(string name, double weight, int whiskersCount)
        {
            Name = name;
            Weight = weight;
            WhiskersCount = whiskersCount;
        }
    }
}
