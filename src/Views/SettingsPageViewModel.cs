using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Glyphy.Views
{
    internal class SettingsPageViewModel : INotifyPropertyChanged
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

        //General.
        public double BrightnessMultiplier { get => _brightnessMultiplier; set => SetProperty(ref _brightnessMultiplier, Math.Clamp(Math.Round(value, 2), 0, 1)); }
        private double _brightnessMultiplier = 0.5;

        //Notification Service.
        public bool IgnorePowerSavings { get => _ignorePowerSavings; set => SetProperty(ref _ignorePowerSavings, value); }
        private bool _ignorePowerSavings = false;
        public bool IgnoreDoNotDisturb { get => _ignoreDoNotDisturb; set => SetProperty(ref _ignoreDoNotDisturb, value); }
        private bool _ignoreDoNotDisturb = false;

        //Ambient Service.
        public bool AmbientServiceEnabled { get => _ambientServiceEnabled; set => SetProperty(ref _ambientServiceEnabled, value); }
        private bool _ambientServiceEnabled = false;

        public double RestartInterval { get => _restartInterval; set => SetProperty(ref _restartInterval, Math.Clamp(Math.Round(value, 2), 0, 1)); }
        private double _restartInterval = 0;
    }
}
