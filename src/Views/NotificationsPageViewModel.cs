using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Glyphy.Views
{
    internal class NotificationsPageViewModel : INotifyPropertyChanged
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

        public bool Enabled { get => _enabled; set => SetProperty(ref _enabled, value); }
        private bool _enabled = false;
    }
}
