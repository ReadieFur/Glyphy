using System;

namespace Glyphy.Misc
{
    public static class Helpers
    {
        //TODO: Implement a way to smooth only the top and end ranges.
        //https://en.wikipedia.org/wiki/Smoothstep
        public static float Smoothstep(float min, float max, float t)
        {
            t = Math.Clamp((t - min) / (max - min), 0.0f, 1.0f);
            return t * t * (3.0f - 2.0f * t);
        }

        public static float SmootherStep(float min, float max, float t)
        {
            t = Math.Clamp((t - min) / (max - min), 0.0f, 1.0f);
            return t * t * t * (3.0f * t * (2.0f * t - 5.0f) + 10.0f);
        }

        //https://stackoverflow.com/questions/929103/convert-a-number-range-to-another-range-maintaining-ratio
        public static double ConvertNumberRange(double oldValue, double oldMin, double oldMax, double newMin, double newMax) =>
            ((oldValue - oldMin) * (newMax - newMin) / (oldMax - oldMin)) + newMin;
    }
}
