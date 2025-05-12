using System.Text.Json.Serialization;

namespace Glyphy.Glyph
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EPhoneType : uint
    {
        Unknown = 0,
        PhoneOne = 20111,
        PhoneTwo = 22111,
        PhoneTwoA = 23111,
        PhoneTwoAPlus = 23113,
        PhoneThreeA = 24111,
        PhoneThreeAPro = 24111
    }
}
