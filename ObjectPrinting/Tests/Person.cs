﻿using System;

namespace ObjectPrinting.Tests
{
    public class Person
    {
        public Person Son { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }

        public object Serialize()
        {
            throw new NotImplementedException();
        }

        public object Serialize(Func<object, object> func)
        {
            throw new NotImplementedException();
        }
    }
}