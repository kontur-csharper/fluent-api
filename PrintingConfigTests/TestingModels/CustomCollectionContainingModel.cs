namespace PrintingConfigTests.TestingModels
{
    public class CustomCollectionContainingModel
    {
        public string String { get; set; }
        public TestingCollection<int> IntCollection { get; set; }
    }
}