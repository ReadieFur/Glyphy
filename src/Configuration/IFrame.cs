using System.Collections.Generic;

namespace Glyphy.Configuration
{
    public interface IFrame
    {
        public uint Frame { get; set; }
        public uint TransitionTime { get; set; }
        public uint Duration { get; set; }
        public IEnumerable<ILEDValue> Values { get; set; }
    }
}
