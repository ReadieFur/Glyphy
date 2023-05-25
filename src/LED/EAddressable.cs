//#define PRETTY_JSON

using System.Text.Json.Serialization;

namespace Glyphy.LED
{
    //[Flags] //Not sure if I can make use of this with my current configuration (the implementation of int).
#if DEBUG || PRETTY_JSON
    [JsonConverter(typeof(JsonStringEnumConverter))]
#endif
    public enum EAddressable //: uint
    {
        CAMERA,
        DIAGONAL,
        RECORDING_LED,
        CENTER_TOP_LEFT,
        CENTER_TOP_RIGHT,
        CENTER_BOTTOM_LEFT,
        CENTER_BOTTOM_RIGHT,
        LINE_1,
        LINE_2,
        LINE_3,
        LINE_4,
        LINE_5,
        LINE_6,
        LINE_7,
        LINE_8,
        DOT
    }
}
