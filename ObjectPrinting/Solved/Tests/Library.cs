﻿using System.Collections.Generic;

namespace ObjectPrinting.Solved.Tests
{
    public class Library
    {
        public Dictionary<string, Book> BooksDictionary = new Dictionary<string, Book>
        {
            ["Alex"] = new Book("Alex", "MyBook")
        };

        public List<Book> BooksList = new List<Book>
        {
            new Book("Alex", "MyBook"),
            new Book("John", "1")
        };

        public Book[] BooksArray { get; set; } =
        {
            new Book("Alex", "MyBook"),
            new Book("John", "1")
        };
    }
}