namespace ObjectPrinting.Tests
{
    public class Computer
    {
        public string GPUName;
        public int RAM;
        public string CPUName { get; set; }

        public Computer(string gpuName, int ram, string cpuName)
        {
            GPUName = gpuName;
            RAM = ram;
            CPUName = cpuName;
        }
    }
}