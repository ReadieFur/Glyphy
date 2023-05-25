using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glyphy.Animation
{
    public struct SAnimation
    {
        [JsonIgnore] public const float MIN_FRAME_RATE = 1;
        [JsonIgnore] public const float MAX_FRAME_RATE = 60;

        //Because GUIDs are timestamp based, it is EXTREMELY unlikely that two GUIDs will be the same so we won't be checking against this.
        [JsonProperty("id")] public Guid Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("frame_rate")] public float FrameRate { get; set; }
        [JsonProperty("frames")] public List<SFrame> Frames { get; set; }

        public static SAnimation CreateNewAnimation()
        {
            return new SAnimation()
            {
                Id = Guid.NewGuid(),
                Name = "Untitled",
                FrameRate = 30, //From my testing 30 seemed reasonable, I could possibly even go down to 15. TODO: Test 15.
                Frames = new List<SFrame>()
                {
                    new()
                    {
                        TransitionTime = 0,
                        Duration = 0,
                        Values = new()
                    }
                }
            };
        }

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
