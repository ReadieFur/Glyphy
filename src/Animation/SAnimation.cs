using Glyphy.Glyph;
using Newtonsoft.Json;

namespace Glyphy.Animation
{
    public struct SAnimation
    {
        [JsonProperty("id")] public Guid Id { get; set; } = Guid.NewGuid();
        [JsonProperty("name")] public string Name { get; set; } = "Untitled";
        [JsonProperty("device")] public EPhoneType PhoneType { get; set; } = EPhoneType.Unknown;
        [JsonProperty("keyframes")] public List<SKeyframe> Keyframes { get; set; } = new();

        public SAnimation() { }
    }
}
