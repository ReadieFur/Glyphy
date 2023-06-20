using Glyphy.LED;

namespace Glyphy.Configuration
{
    public struct SSettings
    {
        public const float BRIGHTNESS_MULTIPLIER_MAX = API.MAX_BRIGHTNESS;
        public const float BRIGHTNESS_MULTIPLIER_MIN = API.MIN_BRIGHTNESS + 0.05f;

        public float BrightnessMultiplier { get; set; } = BRIGHTNESS_MULTIPLIER_MAX;
        public bool IgnoreDoNotDisturb { get; set; } = false;
        public bool IgnorePowerSaverMode { get; set; } = false;
    
        public SSettings() {}
    }
}
