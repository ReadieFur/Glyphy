using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Glyphy.Storage
{
    internal class Settings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool SetProperty<T>(ref T obj, T value, List<string>? additionalTriggers = null, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(obj, value))
                return false;

            obj = value;

            PropertyChanged?.Invoke(this, new(name));
            if (additionalTriggers is not null)
                foreach (string trigger in additionalTriggers)
                    PropertyChanged?.Invoke(this, new(trigger));

            return true;
        }

        [JsonIgnore] public double BrightnessMultiplier { get => _brightnessMultiplier; set => SetProperty(ref _brightnessMultiplier, Math.Clamp(value, 0, 1)); }
        [JsonProperty("brightness_multiplier")] private double _brightnessMultiplier = 1;

        [JsonIgnore] public bool NotificationServiceEnabled { get => _notificationServiceEnabled; set => SetProperty(ref _notificationServiceEnabled, value); }
        [JsonProperty("brightness_multiplier")] private bool _notificationServiceEnabled = false;
        [JsonIgnore] public bool IgnorePowerSavingMode { get => _ignorePowerSavingMode; set => SetProperty(ref _ignorePowerSavingMode, value); }
        [JsonProperty("ignore_power_saving_mode")] private bool _ignorePowerSavingMode = false;
        [JsonIgnore] public bool IgnoreDoNotDisturb { get => _ignoreDoNotDisturb; set => SetProperty(ref _ignoreDoNotDisturb, value); }
        [JsonProperty("ignore_do_not_disturb")] private bool _ignoreDoNotDisturb = false;

        [JsonIgnore] public bool AmbientServiceEnabled { get => _ambientServiceEnabled; set => SetProperty(ref _ambientServiceEnabled, value); }
        [JsonProperty("ambient_service_enabled")] private bool _ambientServiceEnabled = false;
        [JsonIgnore] public double AmbientRestartInterval { get => _ambientRestartInterval; set => SetProperty(ref _ambientRestartInterval, value); }
        [JsonProperty("ambient_restart_interval")] private double _ambientRestartInterval = 0;
        [JsonIgnore] public Guid? AmbientAnimation { get => _ambientAnimation; set => SetProperty(ref _ambientAnimation, value); }
        [JsonProperty("ambient_animation")] private Guid? _ambientAnimation = null;
    }
}
