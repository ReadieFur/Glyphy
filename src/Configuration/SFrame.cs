using Glyphy.LED;
using System.Collections.Generic;

namespace Glyphy.Configuration
{
    public struct SFrame
    {
        public uint Frame { get; set; }
        public uint TransitionTime { get; set; }
        public uint Duration { get; set; }
        public Dictionary<EAddressable, SLEDValue> Values { get; set; }
    }
}
