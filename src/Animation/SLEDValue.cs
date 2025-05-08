using Glyphy.LED;
using Newtonsoft.Json;
using System;

namespace Glyphy.Animation
{
    public struct SLEDValue
    {
        //I will make it a standard that the shared code/API will work in ranges of 0 to 1.
        [JsonIgnore] public const float MIN_BRIGHTNESS = API.MIN_BRIGHTNESS;
        [JsonIgnore] public const float MAX_BRIGHTNESS = API.MAX_BRIGHTNESS;

        [JsonProperty("interpolation_type")] public EInterpolationType InterpolationType { get; set; } = EInterpolationType.NONE;
        [JsonProperty("led")] public ushort Led { get; set; } = ushort.MinValue;
        [JsonProperty("brightness")] public float Brightness { get; set; } = MIN_BRIGHTNESS;

        public SLEDValue() {}

        /// <returns>true if the data was already valid, otherwise false if corrections had to be made.</returns>
        public bool Normalize()
        {
            bool madeCorrections = false;

            //I think the JSON serializer will handle the enum values for us, so we don't need to check them. Either that or it will throw.
            float newBrightness = Math.Clamp(Brightness, MIN_BRIGHTNESS, MAX_BRIGHTNESS);
            madeCorrections |= newBrightness != Brightness;

            return !madeCorrections;
        }
    }
}
