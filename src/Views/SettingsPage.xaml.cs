using Glyphy.Animation;
using Glyphy.Configuration;
using Glyphy.Misc;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Glyphy.Views;

public partial class SettingsPage : ContentPage
{
    private InputControlSyncer<double> brightnessMultiplierInputControlSyncer;
    private SSettings cachedSettings;
    private SAmbientServiceConfiguration cachedAmbientServiceConfiguration;
    private Dictionary<Guid, string> cachedGlyphs = new();

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

        AmbientServiceSwitch.IsEnabled = false;
        AmbientServiceRestartIntervalEntry.IsEnabled = false;
        AmbientServicePicker.IsEnabled = false;

        Task.Run(async () => cachedSettings = await Storage.Settings.GetCached())
            .ContinueWith(_ =>
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

        Task.Run(async () => cachedAmbientServiceConfiguration = await Storage.AmbientService.GetCached())
            .ContinueWith(_ =>
            {
                foreach (Guid glyphID in Storage.GetAnimationIDs())
                {
                    SAnimation? glyph = Storage.LoadAnimation(glyphID).Result;
                    if (glyph is null)
                        continue;

                    cachedGlyphs.Add(glyphID, glyph.Value.Name);
                }

                Dispatcher.Dispatch(() =>
                {
                    AmbientServiceSwitch.IsToggled = cachedAmbientServiceConfiguration.Enabled;
                    AmbientServiceSwitch.IsEnabled = true;

                    AmbientServiceRestartIntervalEntry.Text = Math.Round(cachedAmbientServiceConfiguration.RestartInterval, 1).ToString();
                    AmbientServiceRestartIntervalEntry.IsEnabled = true;

                    AmbientServicePicker.ItemsSource = cachedGlyphs.Values.ToList();
                    AmbientServicePicker.SelectedIndex = cachedGlyphs.Keys.ToList().IndexOf(cachedAmbientServiceConfiguration.AnimationID); //-1 if not found (which is fine).
                    AmbientServicePicker.IsEnabled = true;
                });

                AmbientServiceSwitch.Toggled += AmbientServiceSwitch_Toggled;
                AmbientServiceRestartIntervalEntry.TextChanged += AmbientServiceRestartIntervalEntry_TextChanged;
                AmbientServicePicker.SelectedIndexChanged += AmbientServicePicker_SelectedIndexChanged;
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
        await Task.Run(async () => await Storage.Settings.Save(cachedSettings));
        await Navigation.PopAsync();
    }

    private void AmbientServiceSwitch_Toggled(object? sender, ToggledEventArgs e)
    {
        cachedAmbientServiceConfiguration.Enabled = e.Value;

        Task.Run(async () => await Storage.AmbientService.Save(cachedAmbientServiceConfiguration));
    }

    private void AmbientServiceRestartIntervalEntry_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (!double.TryParse(e.NewTextValue, out double result))
        {
            AmbientServiceRestartIntervalEntry.Text = e.OldTextValue;
            return;
        }

        double clampedValue = Math.Clamp(result, SAmbientServiceConfiguration.RESTART_INTERVAL_MIN, SAmbientServiceConfiguration.RESTART_INTERVAL_MAX);

        cachedAmbientServiceConfiguration.RestartInterval = (float)clampedValue;

        AmbientServiceRestartIntervalEntry.Text = Math.Round(clampedValue, 1).ToString();

        Task.Run(async () => await Storage.AmbientService.Save(cachedAmbientServiceConfiguration));
    }

    private void AmbientServicePicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (AmbientServicePicker.SelectedIndex == -1 || AmbientServicePicker.SelectedIndex >= cachedGlyphs.Count)
            return;

        cachedAmbientServiceConfiguration.AnimationID = cachedGlyphs.Keys.ToList()[AmbientServicePicker.SelectedIndex];

        Task.Run(async () => await Storage.AmbientService.Save(cachedAmbientServiceConfiguration));
    }
}
