using System;
using System.Collections.Generic;
using System.Linq;

namespace ObjectPrinting.Tests
{
    public static class ObjectPrinter_Should_Extensions
    {
        public static IEnumerable<Person> GetRandomBasicPersons(int count = 100)
        {
            for (var i = 0; i < count; i++)
                yield return GetRandomBasicPerson();
        }

        public static Person GetRandomBasicPerson()
        {
            var random = new Random();
            var randomNames = new[] {"Aaron", "Alex", "Bob", "John", "Max", "Dilan", "Bud"};
            
            return new Person
            {
                Name = randomNames[random.Next(randomNames.Length)],
                Age = random.Next(10, 80),
                Height = random.Next(80, 180),
                Id = Guid.NewGuid(),
                SomeArray = null,
                SomeDict = null,
                Mother = null,
                Father = null
            };
        }

        public static IEnumerable<Person> WithRandomParents(this IEnumerable<Person> persons, int maxRecursionLevel = 1)
            => persons.Select(p => p.WithRandomParents(maxRecursionLevel));

        public static Person WithRandomParents(this Person person, int maxRecursionLevel = 1)
        {
            if (maxRecursionLevel == 0) return person;
            var father = GetRandomBasicPerson().WithRandomParents(maxRecursionLevel - 1);
            var mother = GetRandomBasicPerson().WithRandomParents(maxRecursionLevel - 1);
            person.Father = father;
            person.Mother = mother;
            return person;
        }

        public static IEnumerable<Person> WithRandomId(this IEnumerable<Person> persons)
            => persons.Select(WithRandomId);

        public static Person WithRandomId(this Person person)
        {
            person.Id = Guid.NewGuid();
            return person;
        }

        public static IEnumerable<Person> WithNotRoundHeight(this IEnumerable<Person> persons)
            => persons.Select(WithNotRoundHeight);

        public static Person WithNotRoundHeight(this Person person)
        {
            person.Height += new Random().NextDouble();
            return person;
        }

        public static IEnumerable<Person> WithRandomArray(this IEnumerable<Person> persons)
            => persons.Select(WithRandomArray);
        
        public static Person WithRandomArray(this Person person)
        {
            var random = new Random();
            var length = random.Next(10);
            person.SomeArray = new int[length];
            for (var i = 0; i < length; i++)
                person.SomeArray[i] = random.Next(1000);
            return person;
        }
        
        public static IEnumerable<Person> WithRandomList(this IEnumerable<Person> persons)
            => persons.Select(WithRandomList);
        
        public static Person WithRandomList(this Person person)
        {
            var random = new Random();
            var length = random.Next(10);
            person.SomeList = new List<Guid>();
            for (var i = 0; i < length; i++)
                person.SomeList.Add(Guid.NewGuid());
            return person;
        }
        
        public static IEnumerable<Person> WithRandomDictionary(this IEnumerable<Person> persons)
            => persons.Select(WithRandomDictionary);
        
        public static Person WithRandomDictionary(this Person person)
        {
            var keys = new[] {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n"};
            var random = new Random();
            var length = random.Next(10);
            person.SomeDict = new Dictionary<string, int>();
            for (var i = 0; i < length; i++)
                person.SomeDict[keys[random.Next(keys.Length)]] = random.Next(1000);
            return person;
        }
    }
}