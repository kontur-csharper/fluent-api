namespace PrintingConfigTests.TestingModels
{
    public class CascadeModel
    {
        public int Int32 { get; set; }
        public string String { get; set; }
        public CascadeModel Child { get; set; }
    }
}