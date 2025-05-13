namespace Glyphy.Misc
{
    internal static class MathHelpers
    {
        public static double ConvertNumberRange(double oldValue, double oldMin, double oldMax, double newMin, double newMax) =>
            ((oldValue - oldMin) * (newMax - newMin) / (oldMax - oldMin)) + newMin;

        public static bool IsValid(this double self) => !double.IsNaN(self) && !double.IsInfinity(self);

        public static double BezierSolveYGivenX(Point p0, Point p1, Point p2, Point p3, double xTarget)
        {
            double t = SolveBezierT(p0.X, p1.X, p2.X, p3.X, xTarget);
            double u = 1.0 - t;

            double y = u * u * u * p0.Y
                     + 3 * u * u * t * p1.Y
                     + 3 * u * t * t * p2.Y
                     + t * t * t * p3.Y;

            return y;
        }

        //Solves for t′ such that BezierX(t′) ≈ xTarget using binary search.
        private static double SolveBezierT(double x0, double x1, double x2, double x3, double xTarget, int maxIterations = 15, double epsilon = 0.0001)
        {
            double tLow = 0;
            double tHigh = 1;
            double t = 0.5;

            for (int i = 0; i < maxIterations; i++)
            {
                double u = 1 - t;
                double x = u * u * u * x0
                        + 3 * u * u * t * x1
                        + 3 * u * t * t * x2
                        + t * t * t * x3;

                if (Math.Abs(x - xTarget) < epsilon) break;

                if (x < xTarget)
                    tLow = t;
                else
                    tHigh = t;

                t = (tLow + tHigh) * 0.5f;
            }

            return t;
        }
    }
}
