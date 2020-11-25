using System.Globalization;
using NUnit.Framework;
using ObjectPrinting;
using ObjectPrinting.Configuration;
using PrintingConfigTests.TestingModels;

namespace PrintingConfigTests
{
    [TestFixture]
    public class AcceptanceTests
    {
        [Test]
        public void Demo()
        {
            var person = new TestingFieldsClass();

            var result = ObjectPrinter.For<TestingFieldsClass>()
                //1. Исключить из сериализации свойства определенного типа
                .Choose<int>().Exclude()
                //2. Указать альтернативный способ сериализации для определенного типа
                .Choose<double>().UseSerializer(d => $"My beautiful shiny double: {d}")
                //3. Для числовых типов указать культуру
                .Choose(o => o.Int32).SetCulture(CultureInfo.CurrentCulture)
                //4. Настроить сериализацию конкретного свойства
                .Choose(o => o.Guid).UseSerializer(i => i.ToString())
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                .Choose(o => o.String).Trim(10)
                //6. Исключить из сериализации конкретного свойства
                .Choose(o => o.Double).Exclude()
                .PrintToString(person);
            TestContext.Progress.WriteLine(result);

            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию        
            person.Serialize();
            //8. ...с конфигурированием
            new object().Serialize(d => d.Choose<int>().Exclude());
            //Ну после такого можно и бутылочку пива бахнуть
            //Я бы ебнул так то
        }
    }
}