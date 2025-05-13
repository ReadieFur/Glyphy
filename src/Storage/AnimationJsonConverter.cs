using Glyphy.Animation;
using Glyphy.Glyph.Indexes;
using Glyphy.Misc;
using Newtonsoft.Json;

namespace Glyphy.Storage
{
    internal class AnimationJsonConverter : JsonConverter<SAnimation>
    {
        public override SAnimation ReadJson(JsonReader reader, Type objectType, SAnimation existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            SSerializableAnimation intermediate = serializer.Deserialize<SSerializableAnimation>(reader);

            AutoDictionaryInitializer<SPhoneIndex, List<SKeyframe>> keyframes = new();
            foreach (var kvp in intermediate.Keyframes)
            {
                //TODO: Change this so that an animation can have values for a different phone and then during playback filter out for the active device only.
                SPhoneIndex key = new(intermediate.PhoneType, kvp.Key);

                List<SKeyframe> frames = kvp.Value.Select(skf => new SKeyframe
                {
                    Timestamp = skf.Timestamp,
                    Brightness = skf.Brightness,
                    Interpolation = skf.Interpolation,
                    InTangent = skf.InTangent?.Length == 2 ? new Point(skf.InTangent[0], skf.InTangent[1]) : null,
                    OutTangent = skf.OutTangent?.Length == 2 ? new Point(skf.OutTangent[0], skf.OutTangent[1]) : null,
                }).ToList();

                //Sort the keyframes incase the JSON is out of order.
                //Not nessecary/wate of CPU time as sorting will take place again elsewhere in the code.
                //frames.Sort();

                keyframes.Add(key, frames);
            }

            return new SAnimation
            {
                Id = intermediate.Id,
                Name = intermediate.Name,
                PhoneType = intermediate.PhoneType,
                Keyframes = keyframes,
            };
        }

        public override void WriteJson(JsonWriter writer, SAnimation value, JsonSerializer serializer)
        {
            SSerializableAnimation intermediate = new SSerializableAnimation
            {
                Id = value.Id,
                Name = value.Name,
                PhoneType = value.PhoneType, //Uses EnumString converter.
                Keyframes = value.Keyframes
                    .Where(kvp => kvp.Value.Count > 0) //To minimise JSON output, only add entries that contain data.
                    .ToDictionary(
                        kvp => (string)kvp.Key,
                        kvp => kvp.Value
                        .OrderBy(kf => kf) //Sort the keyframes for an organised output (Uses the IComparable on the SKeyframe class).
                        .Select(kf => new SSerializableKeyframe
                        {
                            Timestamp = kf.Timestamp,
                            Brightness = kf.Brightness,
                            Interpolation = kf.Interpolation,
                            InTangent = kf.InTangent is Point inT ? [inT.X, inT.Y] : null,
                            OutTangent = kf.OutTangent is Point outT ? [outT.X, outT.Y] : null,
                        }).ToList()
                    )
            };

            serializer.Serialize(writer, intermediate);
        }
    }
}
