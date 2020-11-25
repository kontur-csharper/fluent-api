namespace PrintingConfigTests.TestingModels
{
    public class NestedContainingModel
    {
        public int Int { get; set; }
        public string String { get; set; }
        public TestingPropertiesClass Nested { get; set; }
    }
}