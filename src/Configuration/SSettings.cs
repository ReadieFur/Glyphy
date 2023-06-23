using Glyphy.LED;
using Newtonsoft.Json;

namespace Glyphy.Configuration
{
    public struct SSettings : IConfigurationBase
    {
        public float Version { get; set; } = 1.0f;

        public const float BRIGHTNESS_MULTIPLIER_MAX = API.MAX_BRIGHTNESS;
        public const float BRIGHTNESS_MULTIPLIER_MIN = API.MIN_BRIGHTNESS + 0.05f;

        [JsonProperty("brightness_multiplier")] public float BrightnessMultiplier { get; set; } = BRIGHTNESS_MULTIPLIER_MAX;
        [JsonProperty("ignore_do_not_disturb")] public bool IgnoreDoNotDisturb { get; set; } = false;
        [JsonProperty("ignore_power_saver_mode")] public bool IgnorePowerSaverMode { get; set; } = false;

        public SSettings() {}
    }
}
