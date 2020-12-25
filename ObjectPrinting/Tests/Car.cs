using System;

namespace ObjectPrinting.Tests
{
    public class Car
    {
        public string Brand { get; set; }
        public string Color { get; set; }
        public string LicensePlateNumber { get; set; }
        public DateTime ReleaseDate { get; set; }

        public Car(string brand, string color, string licensePlateNumber, DateTime releaseDate)
        {
            Brand = brand;
            Color = color;
            LicensePlateNumber = licensePlateNumber;
            ReleaseDate = releaseDate;
        }
    }
}
