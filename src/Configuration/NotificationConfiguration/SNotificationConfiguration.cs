using Newtonsoft.Json;
using System;

namespace Glyphy.Configuration.NotificationConfiguration
{
    public struct SNotificationConfiguration
    {
        [JsonProperty("package_name")] public string PackageName { get; set; }
        [JsonProperty("enabled")]  public bool? Enabled { get; set; } //Null = use default animation if set.
        [JsonProperty("animation_id")] public Guid AnimationID { get; set; }
    }
}
