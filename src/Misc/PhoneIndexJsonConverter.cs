using Glyphy.Glyph;
using Glyphy.Glyph.Indexes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Glyphy.Misc
{
    internal class PhoneIndexJsonConverter : JsonConverter<SPhoneIndex>
    {
        public override SPhoneIndex ReadJson(JsonReader reader, Type objectType, SPhoneIndex existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);

            jObject.TryGetValue("device", out JToken? phoneTypeToken);
            if (phoneTypeToken is null)
                throw new JsonException("Missing target device type field.");

            EPhoneType phoneType;
            try { phoneType = phoneTypeToken.ToObject<EPhoneType>(); }
            catch (Exception ex) { throw new JsonException("Failed to determine device type.", ex); }

            if (reader.Value is not string value)
                throw new JsonException("Value is not of type string.");

            return new SPhoneIndex(phoneType, value);
        }

        public override void WriteJson(JsonWriter writer, SPhoneIndex value, JsonSerializer serializer) => writer.WriteValue((string)value);
    }
}
