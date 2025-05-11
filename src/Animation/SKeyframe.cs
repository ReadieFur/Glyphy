using Newtonsoft.Json;
using System.Numerics;

namespace Glyphy.Animation
{
    public struct SKeyframe
    {
        [JsonProperty("led")] public string Led { get; set; } = string.Empty; //Led codename.
        [JsonProperty("timestamp")] public long Timestamp { get; set; } = 0; //Time in milliseconds
        [JsonProperty("brightness")] public float Brightness { get; set; } = 0; //Brightness 0-1.
        [JsonProperty("interpolation_type")] public EInterpolationType Interpolation { get; set; } = EInterpolationType.None;
        [JsonProperty("in_tangent")] public Vector2? InTangent { get; set; } = new Vector2(0); //Coordinate 0-1 relative to timestamp, brightness and sourrounding frames.
        [JsonProperty("out_tangent")] public Vector2? OutTangent { get; set; } = new Vector2(0); //Same as above.

        public SKeyframe() {}
    }
}
