namespace ObjectPrinting.Tests
{
    public class Book
    {
        public string Author;

        public Book(string author, string name)
        {
            Author = author;
            Name = name;
        }

        public string Name { get; set; }
    }
}
