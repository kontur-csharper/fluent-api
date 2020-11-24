using System;

namespace ObjectPrinterTests
{
    public class PersonWithFriend
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Height { get; set; }
        public int Age { get; set; }

        public PersonWithFriend Friend { get; set; }
    }
}