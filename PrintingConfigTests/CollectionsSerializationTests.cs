using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting;
using PrintingConfigTests.TestingModels;

namespace PrintingConfigTests
{
    public class CollectionsSerializationTests
    {
        private string result;

        [Test]
        public void Array_SerializeEachItem()
        {
            var subject = new ArrayContainingModel
            {
                String = "abc",
                IntArray = Enumerable.Range(0, 5).ToArray()
            };

            result = ObjectPrinter.For<ArrayContainingModel>()
                .Build()
                .PrintToString(subject);

            result.Should()
                .ContainAll(nameof(ArrayContainingModel),
                    $"{nameof(ArrayContainingModel.String)} = {subject.String}",
                    $"{nameof(ArrayContainingModel.IntArray)} = [{subject.IntArray.Length}]",
                    $"[0]:Int32 = {subject.IntArray[0]}",
                    $"[1]:Int32 = {subject.IntArray[1]}",
                    $"[2]:Int32 = {subject.IntArray[2]}",
                    $"[3]:Int32 = {subject.IntArray[3]}",
                    $"[4]:Int32 = {subject.IntArray[4]}");
        }

        [Test]
        public void List_SerializeEachItem()
        {
            var subject = new ListContainingModel
            {
                String = "abc",
                IntList = Enumerable.Range(0, 5).ToList()
            };

            result = ObjectPrinter.For<ListContainingModel>()
                .Build()
                .PrintToString(subject);

            result.Should()
                .ContainAll(nameof(ListContainingModel),
                    $"{nameof(ListContainingModel.String)} = {subject.String}",
                    $"{nameof(ListContainingModel.IntList)} = [{subject.IntList.Count}]",
                    $"[0]:Int32 = {subject.IntList[0]}",
                    $"[1]:Int32 = {subject.IntList[1]}",
                    $"[2]:Int32 = {subject.IntList[2]}",
                    $"[3]:Int32 = {subject.IntList[3]}",
                    $"[4]:Int32 = {subject.IntList[4]}");
        }

        [Test]
        public void CustomCollection_SerializeEachItem()
        {
            var subject = new CustomCollectionContainingModel
            {
                String = "abc",
                IntCollection = new TestingCollection<int>()
            };

            for (var i = 0; i < 5; i++)
                subject.IntCollection.Add(i);

            result = ObjectPrinter.For<CustomCollectionContainingModel>()
                .Build()
                .PrintToString(subject);

            result.Should()
                .ContainAll(nameof(CustomCollectionContainingModel),
                    $"{nameof(CustomCollectionContainingModel.String)} = {subject.String}",
                    $"{nameof(CustomCollectionContainingModel.IntCollection)} = [{subject.IntCollection.Count}]",
                    $"[0]:Int32 = {subject.IntCollection[0]}",
                    $"[1]:Int32 = {subject.IntCollection[1]}",
                    $"[2]:Int32 = {subject.IntCollection[2]}",
                    $"[3]:Int32 = {subject.IntCollection[3]}",
                    $"[4]:Int32 = {subject.IntCollection[4]}");
        }

        [Test]
        public void Dictionary_SerializeEachItem()
        {
            var subject = new DictionaryContainingModel
            {
                String = "abc",
                Dictionary = new Dictionary<int, string>
                {
                    {10, "a"},
                    {20, "b"},
                    {30, "c"},
                    {40, "d"},
                    {50, "e"},
                }
            };

            result = ObjectPrinter.For<DictionaryContainingModel>()
                .Build()
                .PrintToString(subject);

            result.Should()
                .ContainAll(nameof(DictionaryContainingModel),
                    $"{nameof(DictionaryContainingModel.String)} = {subject.String}",
                    $"{nameof(DictionaryContainingModel.Dictionary)} = [{subject.Dictionary.Count}]",
                    $"[10:Int32]:String = {subject.Dictionary[10]}",
                    $"[20:Int32]:String = {subject.Dictionary[20]}",
                    $"[30:Int32]:String = {subject.Dictionary[30]}",
                    $"[40:Int32]:String = {subject.Dictionary[40]}",
                    $"[50:Int32]:String = {subject.Dictionary[50]}");
        }

        [TearDown]
        public void TearDown()
        {
            TestContext.Out.WriteLine(result);
        }
    }
}