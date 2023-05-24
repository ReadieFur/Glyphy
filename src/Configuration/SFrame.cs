using Glyphy.LED;
using System.Collections.Generic;

namespace Glyphy.Configuration
{
    public struct SFrame
    {
        public uint Frame { get; set; }
        public double TransitionTime { get; set; }
        public double Duration { get; set; }
        public Dictionary<EAddressable, SLEDValue> Values { get; set; }
    }
}
