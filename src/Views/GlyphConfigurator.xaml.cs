using Glyphy.Misc;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace Glyphy.Views;

public partial class GlyphConfigurator : ContentPage, IDisposable
{
    //Used to keep the syncer objects in scope.
    private IReadOnlyCollection<object> _inputSyncers;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public GlyphConfigurator()
    {
        SharedConstructor();
	}

    public GlyphConfigurator(Guid id)
    {
        SharedConstructor();
    }
#pragma warning restore CS8618

    public void Dispose()
    {
#if ANDROID
        Android_Dispose();
#endif
    }

    private string EnumKeyToFormattedString(string key)
    {
        string[] parts = key.Split('_');
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = parts[i].ToLower();
            parts[i] = parts[i].Substring(0, 1).ToUpper() + parts[i].Substring(1);
        }
        return string.Join(' ', parts);
    }

    private bool FormattedEnumStringToKey<TEnum>(string str, out TEnum? key) where TEnum : Enum
    {
        string keyString = string.Join('_', str.Split(' ')).ToUpper();

        foreach (string name in Enum.GetNames(typeof(TEnum)))
        {
            if (name == keyString)
            {
                key = (TEnum)Enum.Parse(typeof(TEnum), name);
                return true;
            }
        }

        key = default;
        return false;
    }

    private void SharedConstructor()
    {
        InitializeComponent();

        ToggleControls(false);

        #region Input syncers
        List<object> inputSyncers = new();
        {
            InputControlSyncer<double> inputSyncer = new();
            inputSyncer.AddControl(BrightnessSlider);
            inputSyncer.AddControl(BrightnessEntry);
            inputSyncer.ValueChanged += Brightness_ValueChanged;
            inputSyncers.Add(inputSyncer);
        }
        {
            InputControlSyncer<double> inputSyncer = new();
            inputSyncer.AddControl(TransitionTimeSlider);
            inputSyncer.AddControl(TransitionTimeEntry);
            inputSyncer.ValueChanged += TransitionTime_ValueChanged;
            inputSyncers.Add(inputSyncer);
        }
        {
            InputControlSyncer<double> inputSyncer = new();
            inputSyncer.AddControl(DurationSlider);
            inputSyncer.AddControl(DurationEntry);
            inputSyncer.ValueChanged += Duration_ValueChanged;
            inputSyncers.Add(inputSyncer);
        }
        _inputSyncers = inputSyncers;
        #endregion

        //These get added in order of the enum so we can use the index to get the enum value (though to be safe I will re-format them).
        foreach (string key in Enum.GetNames<LED.EAddressable>())
            LEDPicker.Items.Add(EnumKeyToFormattedString(key));
        LEDPicker.SelectedIndex = 0;

        foreach (string key in Enum.GetNames<Configuration.EInterpolationType>())
            InterpolationPicker.Items.Add(EnumKeyToFormattedString(key));
        InterpolationPicker.SelectedIndex = 0;

        LEDPicker.SelectedIndexChanged += LEDPicker_SelectedIndexChanged;
        InterpolationPicker.SelectedIndexChanged += InterpolationPicker_SelectedIndexChanged;
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        ToggleControls(LED.API.Running);

#if ANDROID
        Android_ContentPage_Loaded(sender, e);
#endif
    }

    private void ToggleControls(bool enabled)
    {
        ControlsGroup.IsEnabled = enabled;
    }

    private void LEDPicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
    }

    private void Brightness_ValueChanged(object? sender, (double, double) e)
    {
    }

    private void InterpolationPicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
    }

    private void TransitionTime_ValueChanged(object? sender, (double, double) e)
    {
    }

    private void Duration_ValueChanged(object? sender, (double, double) e)
    {
    }
}
