using System.Collections.Generic;

namespace Glyphy.Configuration
{
    public struct SFrame
    {
        public uint Frame { get; set; }
        public uint TransitionTime { get; set; }
        public uint Duration { get; set; }
        public IEnumerable<SLEDValue> Values { get; set; }
    }
}
