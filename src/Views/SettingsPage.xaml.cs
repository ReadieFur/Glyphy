using Glyphy.Storage;

namespace Glyphy.Views
{
    public partial class SettingsPage : ContentPage
    {
        private readonly SettingsPageViewModel _viewModel = new();

        public SettingsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel;
            _viewModel.BrightnessMultiplier = StorageManager.Instance.Settings.BrightnessMultiplier;
            _viewModel.IgnorePowerSavings = StorageManager.Instance.Settings.IgnorePowerSavingMode;
            _viewModel.IgnoreDoNotDisturb = StorageManager.Instance.Settings.IgnoreDoNotDisturb;
            _viewModel.AmbientServiceEnabled = StorageManager.Instance.Settings.AmbientServiceEnabled;
            _viewModel.RestartInterval = StorageManager.Instance.Settings.AmbientRestartInterval;

            StoragePath.Text += StorageManager.Instance.ExternalStoragePath;

#if ANDROID
            Android_Constructor();
#endif
        }

        private async void BackButton_Clicked(object? sender, EventArgs e)
        {
            StorageManager.Instance.Settings.BrightnessMultiplier = _viewModel.BrightnessMultiplier;
            StorageManager.Instance.Settings.IgnorePowerSavingMode = _viewModel.IgnorePowerSavings;
            StorageManager.Instance.Settings.IgnoreDoNotDisturb = _viewModel.IgnoreDoNotDisturb;
            StorageManager.Instance.Settings.AmbientServiceEnabled = _viewModel.AmbientServiceEnabled;
            StorageManager.Instance.Settings.AmbientRestartInterval = _viewModel.RestartInterval;
            await StorageManager.Instance.SaveSettings();

            await Navigation.PopAsync();
        }
    }
}
