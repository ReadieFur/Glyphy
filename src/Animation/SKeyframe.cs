using Glyphy.Glyph.Indexes;
using Newtonsoft.Json;
using StringEnumConverter = Newtonsoft.Json.Converters.StringEnumConverter;

namespace Glyphy.Animation
{
    public struct SKeyframe
    {
        [JsonProperty("led")] [JsonConverter(typeof(Misc.PhoneIndexJsonConverter))] public SPhoneIndex Led { get; set; } = default; //Led codename.
        [JsonProperty("timestamp")] public long Timestamp { get; set; } = 0; //Time in milliseconds
        [JsonProperty("brightness")] public double Brightness { get; set; } = 0; //Brightness 0-1.
        [JsonProperty("interpolation_type")] [JsonConverter(typeof(StringEnumConverter))] public EInterpolationType Interpolation { get; set; } = EInterpolationType.None;
        [JsonProperty("in_tangent")] public Point? InTangent { get; set; } = null; //Coordinate 0-1 relative to timestamp, brightness and sourrounding frames.
        [JsonProperty("out_tangent")] public Point? OutTangent { get; set; } = null; //Same as above.

        public SKeyframe() { }
    }
}
