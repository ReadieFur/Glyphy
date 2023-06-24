using System;

namespace Glyphy.Configuration
{
    public struct SAmbientServiceConfiguration : IConfigurationBase
    {
        public const float VERSION = 1.0f;
        public float Version { get; set; } = VERSION;

        public const float RESTART_INTERVAL_MIN = 0.0f;
        public const float RESTART_INTERVAL_MAX = 10.0f;

        public bool Enabled { get; set; } = false;
        public float RestartInterval { get; set; } = RESTART_INTERVAL_MIN;
        public Guid AnimationID { get; set; } = Guid.Empty;

        public SAmbientServiceConfiguration() {}
    }
}
