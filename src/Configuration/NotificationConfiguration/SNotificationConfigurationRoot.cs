using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glyphy.Configuration.NotificationConfiguration
{
    public struct SNotificationConfigurationRoot : IConfigurationBase
    {
        public const float VERSION = 1.0f;
        public float Version { get; set; } = VERSION;

        [JsonProperty("configuration")] public Dictionary<string, SNotificationConfiguration> Configuration { get; set; } = new();

        public SNotificationConfigurationRoot() {}
    }
}
