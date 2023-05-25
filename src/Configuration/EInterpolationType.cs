//#define PRETTY_JSON

using System.Text.Json.Serialization;

namespace Glyphy.Configuration
{
#if DEBUG || PRETTY_JSON
    [JsonConverter(typeof(JsonStringEnumConverter))]
#endif
    public enum EInterpolationType
    {
        NONE,
        LINEAR,
        SMOOTH_STEP,
        SMOOTHER_STEP
    }
}
