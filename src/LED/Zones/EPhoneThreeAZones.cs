using System.Text.Json.Serialization;

namespace Glyphy.LED.Zones
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EPhoneThreeAZones : ushort
    {
        A1 = 20,
        A2 = 21,
        A3 = 22,
        A4 = 23,
        A5 = 24,
        A6 = 25,
        A7 = 26,
        A8 = 27,
        A9 = 28,
        A10 = 29,
        A11 = 30,
        B1 = 31,
        B2 = 32,
        B3 = 33,
        B4 = 34,
        B5 = 35,
        C1 = 0,
        C2 = 1,
        C3 = 2,
        C4 = 3,
        C5 = 4,
        C6 = 5,
        C7 = 6,
        C8 = 7,
        C9 = 8,
        C10 = 9,
        C11 = 10,
        C12 = 11,
        C13 = 12,
        C14 = 13,
        C15 = 14,
        C16 = 15,
        C17 = 16,
        C18 = 17,
        C19 = 18,
        C20 = 19
    }
}
