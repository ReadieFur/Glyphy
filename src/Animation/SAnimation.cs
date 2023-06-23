using Glyphy.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glyphy.Animation
{
    public struct SAnimation : IConfigurationBase
    {
        public float Version { get; set; } = 1.0f;

        [JsonIgnore] public const float MIN_FRAME_RATE = 1;
        [JsonIgnore] public const float MAX_FRAME_RATE = 60;

        //Because GUIDs are timestamp based, it is EXTREMELY unlikely that two GUIDs will be the same so we won't be checking against this.
        [JsonProperty("id")] public Guid Id { get; set; } = Guid.NewGuid();
        [JsonProperty("name")] public string Name { get; set; } = "Untitled";
        [JsonProperty("frame_rate")] public float FrameRate { get; set; } = 30;
        [JsonProperty("frames")] public List<SFrame> Frames { get; set; } = new();

        public SAnimation() {}

        /// <returns>true if the data was already valid, otherwise false if corrections had to be made.</returns>
        public bool Normalize()
        {
            bool madeCorrections = false;

            /*string newName = Name.Substring(0, Math.Min(Name.Length, 20));
            if (newName != Name)
                madeCorrections = true;*/
            float newFrameRate = Math.Clamp(FrameRate, MIN_FRAME_RATE, MAX_FRAME_RATE);
            madeCorrections |= newFrameRate != FrameRate;

            foreach (SFrame frame in Frames)
                madeCorrections |= frame.Normalize();

            return !madeCorrections;
        }
    }
}
