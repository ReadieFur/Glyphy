using System;
using System.Numerics;

namespace Glyphy.Animation
{
    internal class Helpers
    {
        public static float Interpolate(SKeyframe prev, SKeyframe next, long currentTime)
        {
            long delta = next.Timestamp - prev.Timestamp;
            if (delta <= 0) return next.Brightness;

            float t = (currentTime - prev.Timestamp) / (float)delta;
            t = Math.Clamp(t, 0f, 1f);

            switch (next.Interpolation)
            {
                case EInterpolationType.None:
                    return prev.Brightness;
                case EInterpolationType.Linear:
                    return prev.Brightness + t * (next.Brightness - prev.Brightness);
                case EInterpolationType.Smooth:
                    {
                        float u = Math.Clamp((t - prev.Brightness) / (next.Brightness - prev.Brightness), 0.0f, 1.0f);
                        return t * t * t * (3.0f * t * (2.0f * t - 5.0f) + 10.0f);
                    }
                case EInterpolationType.Bezier:
                    {
                        //I'm not even going to pretend I know how this math works, had to ask the computer how to do it, even then after explanation, idk.
                        //I have tested and validated this agaist beziers I had made on Desmos.
                        float x0 = 0f;
                        float x3 = 1f;
                        float y0 = prev.Brightness;
                        float y3 = next.Brightness;

                        Vector2 p0 = new(x0, y0);
                        Vector2 p3 = new(x3, y3);

                        Vector2 rawOut = prev.OutTangent ?? new Vector2(0f, 0f);
                        Vector2 rawIn = next.InTangent ?? new Vector2(1f, 1f);

                        // Map normalized tangents to actual Bezier control points
                        Vector2 p1 = new(x0 + rawOut.X * (x3 - x0), y0 + rawOut.Y * (y3 - y0));
                        Vector2 p2 = new(x0 + rawIn.X * (x3 - x0), y0 + rawIn.Y * (y3 - y0));

                        float tPrime = SolveBezierT(p0.X, p1.X, p2.X, p3.X, t);
                        float u = 1f - tPrime;

                        float y = u * u * u * p0.Y
                                + 3f * u * u * tPrime * p1.Y
                                + 3f * u * tPrime * tPrime * p2.Y
                                + tPrime * tPrime * tPrime * p3.Y;

                        return y;
                    }
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        // Solves for t′ such that BezierX(t′) ≈ xTarget using binary search
        private static float SolveBezierT(float x0, float x1, float x2, float x3, float xTarget, int maxIterations = 15)
        {
            float tLow = 0f;
            float tHigh = 1f;
            float t = 0.5f;

            for (int i = 0; i < maxIterations; i++)
            {
                float u = 1f - t;
                float x = u * u * u * x0
                        + 3f * u * u * t * x1
                        + 3f * u * t * t * x2
                        + t * t * t * x3;

                if (Math.Abs(x - xTarget) < 0.0001f) break;

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
