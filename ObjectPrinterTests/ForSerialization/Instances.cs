using System.Collections.Generic;

namespace ObjectPrinterTests.ForSerialization
{
    public static class Instances
    {
        public static readonly Person Person = new Person {Age = 15, Weight = 67.778, Height = 160.5, Name = "Danil"};

        public static readonly Dictionary<List<int>, int[]> Collection = new Dictionary<List<int>, int[]>
            {[new List<int> {1, 2, 3}] = new[] {5, 7}, [new List<int> {7, 9}] = new[] {10}};

        public static readonly Employee EmployeeWithFewSubordinated = new Employee
        {
            Name = "Ivan",
            Subordinate = new Employee
                {Name = "Jack", Subordinate = new Employee {Name = "Kate", Subordinate = new Employee {Name = "Danil"}}}
        };

        public static readonly Employee EmployeeWithManySubordinates = new Employee
        {
            Name = "1",
            Subordinate = new Employee
            {
                Name = "2",
                Subordinate = new Employee
                {
                    Name = "3",
                    Subordinate = new Employee
                    {
                        Name = "4",
                        Subordinate = new Employee
                        {
                            Name = "5",
                            Subordinate = new Employee
                            {
                                Name = "6",
                                Subordinate = new Employee
                                {
                                    Name = "7",
                                    Subordinate = new Employee
                                    {
                                        Name = "8",
                                        Subordinate = new Employee
                                        {
                                            Name = "9",
                                            Subordinate = new Employee
                                            {
                                                Name = "10",
                                                Subordinate = new Employee {Name = "11", Subordinate = null}
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}