namespace ObjectPrinting.Tests
{
    public class Teacher
    {
        public Teacher(string name, string position, Student bestStudent)
        {
            Name = name;
            BestStudent = bestStudent;
            Position = position;
        }


        public string Name { get; set; }
        public string Position { get; set; }
        public Student BestStudent { get; set; }
    }
}