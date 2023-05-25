using Glyphy.LED;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glyphy.Configuration
{
    public struct SFrame
    {
        [JsonProperty("frame")] public uint Frame { get; set; }
        [JsonProperty("transition_time")] public double TransitionTime { get; set; }
        [JsonProperty("duration")] public double Duration { get; set; }
        [JsonProperty("values")] public Dictionary<EAddressable, SLEDValue> Values { get; set; }
    }
}
