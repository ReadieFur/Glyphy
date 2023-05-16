using System;

namespace Glyphy
{
    //[Flags] //Not sure if I can make use of this with my current configuration (the implementation of int).
    public enum ELEDAddressable //: uint
    {
        DIAGONAL,
        CENTER_BOTTOM_LEFT,
        CENTER_BOTTOM_RIGHT,
        CENTER_TOP_LEFT,
        CENTER_TOP_RIGHT,
        CAMERA,
        LINE_1,
        LINE_2,
        LINE_3,
        LINE_4,
        LINE_5,
        LINE_6,
        LINE_7,
        LINE_8,
        DOT,
        RECORDING_LED
    }
}
