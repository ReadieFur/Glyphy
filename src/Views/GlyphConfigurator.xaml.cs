using Glyphy.Configuration;
using Glyphy.LED;
using Glyphy.Misc;
using Microsoft.Maui.Controls;
using System;

namespace Glyphy.Views;

public partial class GlyphConfigurator : ContentPage, IDisposable
{
    #region Properties
    private InputControlSyncer<double> brightnessInputControlSyncer;
    private InputControlSyncer<double> transitionTimeInputControlSyncer;
    private InputControlSyncer<double> durationInputControlSyncer;

    //TODO: Have the save icon go from inactive to active when there are unsaved changes.
    private bool hasUnsavedChanges = false;
    //TODO: Have the back icon go red when the ability to discard changes is available.
    private bool canDiscardChanges = false;
    private SAnimation animation;
    private int currentFrameIndex = 0;
    private EAddressable selectedLED = EAddressable.CAMERA;
    #endregion

    #region Constructors
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public GlyphConfigurator()
    {
        animation = SAnimation.CreateNewAnimation();

        SharedConstructor();
	}

    public GlyphConfigurator(Guid id)
    {
        //TODO: Move this out of the constructor as it contains long running code (async methods).
        SAnimation? _animation = Storage.LoadAnimation(id).Result;
        if (_animation == null)
        {
#if ANDROID
            Android.Widget.Toast.MakeText(Android.App.Application.Context, "Failed to load animation.", Android.Widget.ToastLength.Long)?.Show();
#endif

            Navigation.PopModalAsync().Wait();
            return;
        }

        SharedConstructor();
    }
#pragma warning restore CS8618

    public void Dispose()
    {
#if ANDROID
        Android_Dispose();
#endif
    }

    //TODO: Speed up this constructor as it takes too long to load.
    private void SharedConstructor()
    {
        InitializeComponent();

        ToggleControls(false);

        //These get added in order of the enum so we can use the index to get the enum value (though to be safe I will re-format them).
        foreach (string key in Enum.GetNames<EAddressable>())
        {
            GlyphPreview.UpdatePreview(Enum.Parse<EAddressable>(key), 0);
            LEDPicker.Items.Add(EnumKeyToFormattedString(key));
        }
        LEDPicker.SelectedIndex = 0;

        foreach (string key in Enum.GetNames<EInterpolationType>())
            InterpolationPicker.Items.Add(EnumKeyToFormattedString(key));
        InterpolationPicker.SelectedIndex = 0;

        AnimationNameEntry.Text = animation.Name;

        #region Input syncers
        brightnessInputControlSyncer = new();
        brightnessInputControlSyncer.AddControl(BrightnessSlider);
        brightnessInputControlSyncer.AddControl(BrightnessEntry);
        brightnessInputControlSyncer.ValueChanged += Brightness_ValueChanged;

        transitionTimeInputControlSyncer = new();
        transitionTimeInputControlSyncer.AddControl(TransitionTimeSlider);
        transitionTimeInputControlSyncer.AddControl(TransitionTimeEntry);
        transitionTimeInputControlSyncer.ValueChanged += Transition_ValueChanged;

        durationInputControlSyncer = new();
        durationInputControlSyncer.AddControl(DurationSlider);
        durationInputControlSyncer.AddControl(DurationEntry);
        durationInputControlSyncer.ValueChanged += Duration_ValueChanged;
        #endregion

        LEDPicker.SelectedIndexChanged += LEDPicker_SelectedIndexChanged;
    }
    #endregion

    #region Helpers
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

    //I can't use this right now as I can't seem to get the selected item string from the picker.
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

    private EAddressable? GetSelectedAddressableLED()
    {
        string[] enumNames = Enum.GetNames(typeof(EAddressable));
        if (LEDPicker.SelectedIndex < 0 || LEDPicker.SelectedIndex >= enumNames.Length)
            return null;
        return Enum.Parse<EAddressable>(enumNames[LEDPicker.SelectedIndex]);
    }

    private void ToggleControls(bool enabled)
    {
        ControlsGroup.IsEnabled = enabled;
    }
    #endregion

    #region Event handlers
    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        ToggleControls(API.Running);

#if ANDROID
        Android_ContentPage_Loaded(sender, e);
#endif
    }

    private void LEDPicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        hasUnsavedChanges = true;
    }

    private void Brightness_ValueChanged(object? sender, (double, double) e)
    {
        hasUnsavedChanges = true;

        #region Format inputs
        brightnessInputControlSyncer.Update(Math.Round(e.Item1, 1));
        #endregion

        #region Update configuration
        EAddressable? selectedLED = GetSelectedAddressableLED();
        if (selectedLED is null)
            return;

        SLEDValue entry;
        if (!animation.Frames[currentFrameIndex].Values.ContainsKey(selectedLED.Value))
        {
            entry = new();
            entry.InterpolationType = EInterpolationType.NONE;
            entry.Led = selectedLED.Value;
        }
        else
        {
            entry = animation.Frames[currentFrameIndex].Values[selectedLED.Value];
        }

        entry.Brightness = Helpers.ConvertNumberRange(e.Item1, 0, 100, 0, 1);

        animation.Frames[currentFrameIndex].Values[selectedLED.Value] = entry;
        #endregion

        #region Update live preview.
        GlyphPreview.UpdatePreview(selectedLED.Value, e.Item1);
        #endregion

        #region Update physical LED
        if (API.Running)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            API.Instance.SetBrightness(selectedLED.Value,
                API.Instance.ClampBrightness((int)Math.Round(Helpers.ConvertNumberRange(e.Item1, 0, 100, 0, API.Instance.MaxBrightness))));
#pragma warning restore CS4014
        }
        #endregion
    }

    private void Transition_ValueChanged(object? sender, (double, double) e)
    {
        hasUnsavedChanges = true;
        transitionTimeInputControlSyncer.Update(Math.Round(e.Item1, 1));
    }

    private void Duration_ValueChanged(object? sender, (double, double) e)
    {
        hasUnsavedChanges = true;
        durationInputControlSyncer.Update(Math.Round(e.Item1, 1));
    }

    private void SaveButton_Clicked(object sender, EventArgs e)
    {
        hasUnsavedChanges = false;
        canDiscardChanges = false;
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        if (!OnBackButtonPressed())
            Navigation.PopAsync();
    }
    #endregion

    #region Overrides
    protected override bool OnBackButtonPressed()
    {
        if (canDiscardChanges)
            return false;

        canDiscardChanges = true;

#if ANDROID
        Android.Widget.Toast.MakeText(Android.App.Application.Context, "Unsaved changes.\nPress again to discard changes.", Android.Widget.ToastLength.Long)?.Show();
#endif

        return true;
    }
    #endregion
}
