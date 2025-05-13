using Glyphy.Glyph;
using Newtonsoft.Json;
using StringEnumConverter = Newtonsoft.Json.Converters.StringEnumConverter;

namespace Glyphy.Storage
{
    internal readonly struct SSerializableAnimation
    {
        //Despite these being init only, reflection can still set the values so the Json deserializer will still work.
        [JsonProperty("id")] public required Guid Id { get; init; }
        [JsonProperty("name")] public required string Name { get; init; }
        [JsonProperty("device")][JsonConverter(typeof(StringEnumConverter))] public required EPhoneType PhoneType { get; init; }
        [JsonProperty("keyframes")] public required Dictionary<string, List<SSerializableKeyframe>> Keyframes { get; init; }
    }
}
