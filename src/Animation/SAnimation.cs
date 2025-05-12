using Glyphy.Glyph;
using Newtonsoft.Json;
using StringEnumConverter = Newtonsoft.Json.Converters.StringEnumConverter;

namespace Glyphy.Animation
{
    public struct SAnimation
    {
        [JsonProperty("id")] public Guid Id { get; set; } = Guid.NewGuid();
        [JsonProperty("name")] public string Name { get; set; } = "Untitled";
        [JsonProperty("device")] [JsonConverter(typeof(StringEnumConverter))] public EPhoneType PhoneType { get; set; } = EPhoneType.Unknown;
        [JsonProperty("keyframes")] public List<SKeyframe> Keyframes { get; set; } = new();

        public SAnimation() { }
    }
}
