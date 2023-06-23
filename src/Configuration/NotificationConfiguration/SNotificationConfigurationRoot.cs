using Newtonsoft.Json;
using System.Collections.Generic;

namespace Glyphy.Configuration.NotificationConfiguration
{
    public struct SNotificationConfigurationRoot : IConfigurationBase
    {
        public float Version { get; set; } = 1.0f;
        [JsonProperty("configuration")] public Dictionary<string, SNotificationConfiguration> Configuration { get; set; } = new();

        public SNotificationConfigurationRoot() {}
    }
}
