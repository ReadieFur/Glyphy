using Glyphy.Configuration;
using Glyphy.Misc;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace Glyphy.Views;

public partial class SettingsPage : ContentPage
{
    private InputControlSyncer<double> brightnessMultiplierInputControlSyncer;
    private SSettings cachedSettings;

    public SettingsPage()
    {
        InitializeComponent();

        StoragePathLabel.Text = $"Storage Path: {Storage.BasePath}";
        //TODO: Make this path clickable.
        //Launcher.OpenAsync(new OpenFileRequest { File = new Microsoft.Maui.Storage.ReadOnlyFile(Storage.BasePath) });

        brightnessMultiplierInputControlSyncer = new();
        brightnessMultiplierInputControlSyncer.AddControl(BrightnessMultiplierSlider);
        brightnessMultiplierInputControlSyncer.AddControl(BrightnessMultiplierEntry);
        brightnessMultiplierInputControlSyncer.ControlsEnabled = false;
        BrightnessMultiplierSlider.Minimum = SSettings.BRIGHTNESS_MULTIPLIER_MIN;
        BrightnessMultiplierSlider.Maximum = SSettings.BRIGHTNESS_MULTIPLIER_MAX;

        IgnorePowerSavingModeSwitch.IsEnabled = false;
        IgnoreDoNotDisturbSwitch.IsEnabled = false;

        Task.Run(async () => cachedSettings = await Storage.GetCachedSettings())
            .ContinueWith((res) =>
            {
                double brightnessValue = Math.Round(cachedSettings.BrightnessMultiplier, 1);

                Dispatcher.Dispatch(() =>
                {
                    brightnessMultiplierInputControlSyncer.SetValue(brightnessValue);
                    brightnessMultiplierInputControlSyncer.ControlsEnabled = true;

                    IgnorePowerSavingModeSwitch.IsToggled = cachedSettings.IgnorePowerSaverMode;
                    IgnorePowerSavingModeSwitch.IsEnabled = true;

                    IgnoreDoNotDisturbSwitch.IsToggled = cachedSettings.IgnoreDoNotDisturb;
                    IgnoreDoNotDisturbSwitch.IsEnabled = true;
                });

                brightnessMultiplierInputControlSyncer.ValueChanged += BrightnessMultiplier_ValueChanged;
                IgnorePowerSavingModeSwitch.Toggled += IgnorePowerSavingModeSwitch_Toggled;
                IgnoreDoNotDisturbSwitch.Toggled += IgnoreDoNotDisturbSwitch_Toggled;
            });
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
#if ANDROID
        Android_ContentPage_Loaded(sender, e);
#endif
    }

    private double? BrightnessMultiplier_ValueChanged(double newValue, object? sender)
    {
        double clampedValue = Math.Clamp(newValue, SSettings.BRIGHTNESS_MULTIPLIER_MIN, SSettings.BRIGHTNESS_MULTIPLIER_MAX);
        double roundedValue = Math.Round(clampedValue, 2);

        cachedSettings.BrightnessMultiplier = (float)clampedValue;

        return roundedValue;
    }

    private void IgnorePowerSavingModeSwitch_Toggled(object? sender, ToggledEventArgs e) =>
        cachedSettings.IgnorePowerSaverMode = e.Value;

    private void IgnoreDoNotDisturbSwitch_Toggled(object? sender, ToggledEventArgs e) =>
        cachedSettings.IgnoreDoNotDisturb = e.Value;

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        //Is this task wrapping necessary?
        await Task.Run(async () => await Storage.SaveSettings(cachedSettings));
        await Navigation.PopAsync();
    }
}
