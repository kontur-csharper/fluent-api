namespace PrintingConfigTests.TestingModels
{
    public class SeveralNestedContainingModel
    {
        public string String { get; set; }
        public SeveralNestedContainingModel M1 { get; set; }
        public SeveralNestedContainingModel M2 { get; set; }
    }
}