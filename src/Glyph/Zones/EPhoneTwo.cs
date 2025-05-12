using System.Text.Json.Serialization;

namespace Glyphy.Glyph.Zones
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EPhoneTwo : ushort
    {
        A1 = 0,
        A2 = 1,
        B1 = 2,
        C1_1 = 3,
        C1_2 = 4,
        C1_3 = 5,
        C1_4 = 6,
        C1_5 = 7,
        C1_6 = 8,
        C1_7 = 9,
        C1_8 = 10,
        C1_9 = 11,
        C1_10 = 12,
        C1_11 = 13,
        C1_12 = 14,
        C1_13 = 15,
        C1_14 = 16,
        C1_15 = 17,
        C1_16 = 18,
        C2 = 19,
        C3 = 20,
        C4 = 21,
        C5 = 22,
        C6 = 23,
        D1_1 = 25,
        D1_2 = 26,
        D1_3 = 27,
        D1_4 = 28,
        D1_5 = 29,
        D1_6 = 30,
        D1_7 = 31,
        D1_8 = 32,
        E1 = 24
    }
}
