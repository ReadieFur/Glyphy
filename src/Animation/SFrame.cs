using Glyphy.LED;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glyphy.Animation
{
    public struct SFrame
    {
        [JsonIgnore] public const float MIN_TRANSITION_TIME = 0;
        [JsonIgnore] public const float MAX_TRANSITION_TIME = 5;
        [JsonIgnore] public const float MIN_DURATION = 0;
        [JsonIgnore] public const float MAX_DURATION = 1;

        [JsonProperty("transition_time")] public float TransitionTime { get; set; } = MIN_TRANSITION_TIME;
        [JsonProperty("duration")] public float Duration { get; set; } = MIN_DURATION;
        [JsonProperty("values")] public Dictionary<string, SLEDValue> Values { get; set; } = new();

        public SFrame() {}

        /// <returns>true if the data was already valid, otherwise false if corrections had to be made.</returns>
        public bool Normalize()
        {
            bool madeCorrections = false;

            float newTransitionTime = Math.Clamp(TransitionTime, MIN_TRANSITION_TIME, MAX_TRANSITION_TIME);
            madeCorrections |= newTransitionTime != TransitionTime;

            foreach (KeyValuePair<string, SLEDValue> kvp in Values)
            {
                bool madeLEDCorrections = kvp.Value.Normalize();
                madeCorrections |= madeLEDCorrections;

                if (madeLEDCorrections)
                    Values[kvp.Key] = kvp.Value;
            }

            return !madeCorrections;
        }
    }
}
