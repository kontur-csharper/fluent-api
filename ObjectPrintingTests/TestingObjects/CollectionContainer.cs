using System;
using System.Collections.Generic;

namespace ObjectPrintingTests.TestingObjects
{
    public class CollectionContainer
    {
        public string[] Addresses { get; } = {"A st.", "Z st."};

        public Dictionary<int, int> Numbers { get; } = new Dictionary<int, int>
        {
            {1, 2}, {10, 30}
        };

        public DateTime CurrentTime => DateTime.Now;
    }
}
