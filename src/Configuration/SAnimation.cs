using System;
using System.Collections.Generic;

namespace Glyphy.Configuration
{
    public struct SAnimation
    {
        //Because GUIDs are timestamp based, it is EXTREMELY unlikely that two GUIDs will be the same so we won't be checking against this.
        public Guid Id { get; set; }
        public string Name { get; set; }
        public uint FrameRate { get; set; }
        public IEnumerable<SFrame> Frames { get; set; }
    }
}
