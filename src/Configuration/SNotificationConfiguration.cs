using System;

namespace Glyphy.Configuration
{
    public struct SNotificationConfiguration
    {
        public string PackageName { get; set; }
        public bool? Enabled { get; set; } //Null = use default animation if set.
        public Guid AnimationID { get; set; }
    }
}
