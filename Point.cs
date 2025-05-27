using System;

namespace CourseWorkRealease
{
    internal class Points
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int Length { get; internal set; }

        public Points()
        {
            X = 0;
            Y = 0;
        }
        public Points(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
