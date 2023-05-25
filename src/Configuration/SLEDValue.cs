using Glyphy.LED;
using Newtonsoft.Json;

namespace Glyphy.Configuration
{
    public struct SLEDValue
    {
        [JsonProperty("interpolation_type")] public EInterpolationType InterpolationType { get; set; }
        [JsonProperty("led")] public EAddressable Led { get; set; }
        [JsonProperty("brightness")] public double Brightness { get; set; }
    }
}
