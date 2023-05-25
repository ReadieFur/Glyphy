using System;

namespace Glyphy.Animation
{
    internal class Interpolation
    {
        //public delegate float InterpolationDelegate(float min, float max, float delta);

        public static float Linear(float min, float max, float delta)
        {
            return min + (max - min) * delta;
        }

        //TODO: Implement a way to smooth only the top and end ranges.
        //https://en.wikipedia.org/wiki/Smoothstep
        public static float Smoothstep(float min, float max, float delta)
        {
            delta = Math.Clamp((delta - min) / (max - min), 0.0f, 1.0f);
            return delta * delta * (3.0f - 2.0f * delta);
        }

        public static float SmootherStep(float min, float max, float delta)
        {
            delta = Math.Clamp((delta - min) / (max - min), 0.0f, 1.0f);
            return delta * delta * delta * (3.0f * delta * (2.0f * delta - 5.0f) + 10.0f);
        }
    }
}
