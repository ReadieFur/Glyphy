using System.Text.Json.Serialization;

namespace Glyphy.Animation
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EInterpolationType
    {
        None, //Instant
        Linear,
        Smooth,
        Bezier
    }
}
