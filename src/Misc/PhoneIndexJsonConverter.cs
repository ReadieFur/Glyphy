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
            /*JObject jObject = JObject.Load(reader);

            jObject.TryGetValue("device", out JToken? phoneTypeToken);
            if (phoneTypeToken is null)
                throw new JsonException("Missing target device type field.");

            EPhoneType phoneType;
            try { phoneType = phoneTypeToken.ToObject<EPhoneType>(); }
            catch (Exception ex) { throw new JsonException("Failed to determine device type.", ex); }

            if (reader.Value is not string value)
                throw new JsonException("Value is not of type string.");

            return new SPhoneIndex(phoneType, value);*/

            if (reader.Value is not string value)
                throw new JsonException("Value is not of type string.");

            string[] parts = value.Split('.', 2);
            if (parts.Length != 2 || !uint.TryParse(parts[0], out uint phoneTypeId))
                throw new JsonException("Unable to determine device type.");

            if (!Enum.IsDefined(typeof(EPhoneType), phoneTypeId))
                throw new JsonException("Invalid device type.");

            return new((EPhoneType)phoneTypeId, parts[1]);
        }

        public override void WriteJson(JsonWriter writer, SPhoneIndex value, JsonSerializer serializer)
        {
            //TODO: Temporary solution while I figure out how to read the device type from the root of the tree.
            writer.WriteValue($"{(uint)value.PhoneType}.{value.Key}");
        }
    }
}
