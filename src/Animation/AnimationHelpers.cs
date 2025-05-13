using Glyphy.Misc;

namespace Glyphy.Animation
{
    internal class AnimationHelpers
    {
        public static double Interpolate(SKeyframe prev, SKeyframe next, double currentTime)
        {
            long delta = next.Timestamp - prev.Timestamp;
            if (delta <= 0)
                return next.Brightness;

            double t = Math.Clamp((currentTime - prev.Timestamp) / delta, 0, 1);

            switch (next.Interpolation)
            {
                case EInterpolationType.None:
                    return prev.Brightness;
                case EInterpolationType.Linear:
                    return prev.Brightness + t * (next.Brightness - prev.Brightness);
                case EInterpolationType.Smooth:
                    {
                        //float eased = t * t * (3f - 2f * t); //Smooth step.
                        double eased = t * t * t * (t * (6f * t - 15f) + 10f); //Smoother step.
                        return prev.Brightness + eased * (next.Brightness - prev.Brightness);
                    }
                case EInterpolationType.Bezier:
                    {
                        double x0 = 0;
                        double x3 = 1;
                        double y0 = prev.Brightness;
                        double y3 = next.Brightness;

                        Point p0 = new(x0, y0);
                        Point p3 = new(x3, y3);

                        Point rawOut = prev.OutTangent ?? new Point(0, 0);
                        Point rawIn = next.InTangent ?? new Point(0, 0);

                        //Map normalized tangents to actual Bezier control points relative to the previous and next timestamps.
                        Point p1 = new(x0 + rawOut.X * (x3 - x0), y0 + rawOut.Y * (y3 - y0));
                        Point p2 = new(x0 + rawIn.X * (x3 - x0), y0 + rawIn.Y * (y3 - y0));

                        double y = MathHelpers.BezierSolveYGivenX(p0, p1, p2, p3, t);
                        
                        return y;
                    }
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }
}
