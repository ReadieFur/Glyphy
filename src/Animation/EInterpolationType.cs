#define PRETTY_JSON

using System.Text.Json.Serialization;

namespace Glyphy.Animation
{
#if DEBUG || PRETTY_JSON
    [JsonConverter(typeof(JsonStringEnumConverter))]
#endif
    public enum EInterpolationType
    {
        None, //Instant
        Linear,
        Smooth,
        Bezier
    }
}
