namespace ObjectPrinting.Tests
{
    public class Human
    {
        public Human(string name, Human bestFriend, Human friend)
        {
            Name = name;
            BestFriend = bestFriend;
            Friend = friend;
        }

        public string Name { get; set; }
        public Human BestFriend { get; set; }
        public Human Friend { get; set; }
    }
}