using System;

namespace CourseWorkRealease
{
    internal class Interpolation
    {
        public static (double, int) LagrangeInterpolation(Points[] points, double xValue)
        {
            int n = points.Length;
            double result = 0;
            int iterations = 0;

            for (int i = 0; i < n; i++)
            {
                double li = 1.0;
                for (int j = 0; j < n; j++)
                {
                    if (j != i)
                    {
                        double denominator = points[i].X - points[j].X;
                        li *= (xValue - points[j].X) / denominator;
                        iterations++;
                    }
                }
                result += points[i].Y * li;
                iterations++;
            }
            return (result, iterations);
        }
        public static (double, int) AitkenInterpolation(Points[] points, double xValue)
        {
            int n = points.Length;
            double[,] P = new double[n, n];
            int iterations = 0;

            for (int i = 0; i < n; i++)
            {
                P[i, 0] = points[i].Y;
            }

            for (int j = 1; j < n; j++)
            {
                for (int i = j; i < n; i++)
                {
                    double xi = points[i].X;
                    double xj = points[i - j].X;
                    double denominator = xi - xj;
                    P[i, j] = ((xValue - xj) * P[i, j - 1] - (xValue - xi) * P[i - 1, j - 1]) / denominator;
                    iterations++;
                }
            }

            return (P[n - 1, n - 1], iterations);
        }
    }
}
