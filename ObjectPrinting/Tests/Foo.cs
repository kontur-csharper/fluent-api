namespace ObjectPrinting.Tests
{
    public class Foo
    {
        public string Name { get; set; }
        public Foo Next { get; set; }

        protected bool Equals(Foo other) => Name == other.Name;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Foo) obj);
        }

        public override int GetHashCode() => (Name != null ? Name.GetHashCode() : 0);
    }
}