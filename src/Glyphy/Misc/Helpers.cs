using System;

namespace Glyphy.Misc
{
    public static class Helpers
    {
        //https://stackoverflow.com/questions/929103/convert-a-number-range-to-another-range-maintaining-ratio
        public static double ConvertNumberRange(double oldValue, double oldMin, double oldMax, double newMin, double newMax) =>
            ((oldValue - oldMin) * (newMax - newMin) / (oldMax - oldMin)) + newMin;

        public static float ConvertNumberRange(float oldValue, float oldMin, float oldMax, float newMin, float newMax) =>
            ((oldValue - oldMin) * (newMax - newMin) / (oldMax - oldMin)) + newMin;
    }
}
