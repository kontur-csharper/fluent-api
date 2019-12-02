﻿using System;
using System.Collections.Generic;

namespace ObjectPrinting.Tests
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }

        public Person Boss { get; set; }
        public List<Person> Family { get; set; }

        public DateTime BirthDay { get; set; }
    }
}