using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glyphy.Configuration
{
    public struct SAnimation
    {
        //Because GUIDs are timestamp based, it is EXTREMELY unlikely that two GUIDs will be the same so we won't be checking against this.
        [JsonProperty("id")] public Guid Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("frame_rate")] public uint FrameRate { get; set; }
        [JsonProperty("frames")] public List<SFrame> Frames { get; set; }

        public static SAnimation CreateNewAnimation()
        {
            return new SAnimation()
            {
                Id = Guid.NewGuid(),
                Name = "Untitled",
                FrameRate = 60,
                Frames = new List<SFrame>()
                {
                    new()
                    {
                        Frame = 0,
                        TransitionTime = 0,
                        Duration = 0,
                        Values = new()
                    }
                }
            };
        }
    }
}
