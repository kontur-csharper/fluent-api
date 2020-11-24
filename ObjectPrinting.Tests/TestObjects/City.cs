using System.Collections.Generic;

namespace ObjectPrinting.Tests.TestObjects
{
    public class City
    {
        public City()
        {
            RoadsToCity = new Dictionary<int, City>();
        }

        public string Name { get; set; }
        public int Population { get; set; }
        public Dictionary<int, City> RoadsToCity { get; }
    }
}