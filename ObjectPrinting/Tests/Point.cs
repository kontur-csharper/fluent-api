namespace ObjectPrinting.Tests
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public float Z { get; set; }

        public Point(double x, double y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point()
        {
        }
    }
}