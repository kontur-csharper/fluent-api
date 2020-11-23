using System.Collections.Generic;

namespace ObjectPrinting.Tests
{
    public class Library
    {
        public Dictionary<string, Book> BooksDictionary = new Dictionary<string, Book>
        {
            ["Alex"] = new Book("Alex", "MyBook")
        };

        public List<Book> BooksList = new List<Book>
        {
            new Book("Alex", "MyBook")
        };

        public Book[] BooksArray { get; set; } =
        {
            new Book("Alex", "MyBook")
        };
    }
}
