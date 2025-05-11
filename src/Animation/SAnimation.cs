using Glyphy.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Glyphy.Animation
{
    public struct SAnimation : IConfigurationBase
    {
        public float Version { get; set; } = 2.0f;

        //Because GUIDs are timestamp based, it is EXTREMELY unlikely that two GUIDs will be the same so we won't be checking against this.
        [JsonProperty("id")] public Guid Id { get; set; } = Guid.NewGuid();
        [JsonProperty("name")] public string Name { get; set; } = "Untitled";
        [JsonProperty("frame_rate")] public ushort FrameRate { get; set; } = 30;
        [JsonProperty("keyframes")] public List<SKeyframe> Keyframes { get; set; } = new();

        public SAnimation() {}
    }
}
