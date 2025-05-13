using Glyphy.Animation;
using Newtonsoft.Json;
using StringEnumConverter = Newtonsoft.Json.Converters.StringEnumConverter;

namespace Glyphy.Storage
{
    internal readonly struct SSerializableKeyframe
    {
        [JsonProperty("timestamp")] public required long Timestamp { get; init; }
        [JsonProperty("brightness")] public required double Brightness { get; init; }
        [JsonProperty("interpolation_type")][JsonConverter(typeof(StringEnumConverter))] public EInterpolationType Interpolation { get; init; }
        [JsonProperty("in_tangent")] public double[]? InTangent { get; init; }
        [JsonProperty("out_tangent")] public double[]? OutTangent { get; init; }
    }
}
