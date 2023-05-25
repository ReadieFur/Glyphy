using Glyphy.Animation;
using Glyphy.Configuration;
using Glyphy.LED;
using Glyphy.Misc;
using Microsoft.Maui.Controls;
using System;
using System.IO;

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
    public SAnimation animation { get; private set; }
    private int currentFrameIndex = 0;
    private EAddressable selectedLED = EAddressable.CAMERA;
    #endregion

    #region Constructors
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public GlyphConfigurator()
    {
        //Set to true for new animations.
        hasUnsavedChanges = true;
        animation = SAnimation.CreateNewAnimation();

        SharedConstructor();
	}

    public GlyphConfigurator(Guid id)
    {
        //TODO: Move this out of the constructor as it contains long running code (async methods).
        SAnimation? animation = Storage.LoadAnimation(id).Result;
        if (animation is null)
            throw new IOException("Failed to load animation.");
        this.animation = animation.Value;

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

        TransitionTimeSlider.Minimum = SFrame.MIN_TRANSITION_TIME;
        TransitionTimeSlider.Maximum = SFrame.MAX_TRANSITION_TIME;

        DurationSlider.Minimum = SFrame.MIN_DURATION;
        DurationSlider.Maximum = SFrame.MAX_DURATION;

        #region Input syncers
        brightnessInputControlSyncer = new();
        brightnessInputControlSyncer.AddControl(BrightnessSlider);
        brightnessInputControlSyncer.AddControl(BrightnessEntry);
        brightnessInputControlSyncer.ValueChanged += Brightness_ValueChanged;

        transitionTimeInputControlSyncer = new();
        transitionTimeInputControlSyncer.AddControl(TransitionTimeSlider);
        transitionTimeInputControlSyncer.AddControl(TransitionTimeEntry);
        transitionTimeInputControlSyncer.ValueChanged += TransitionTime_ValueChanged;

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

    private SLEDValue? GetCurrentLEDConfiguration()
    {
        EAddressable? selectedLED = GetSelectedAddressableLED();
        if (selectedLED is null)
            return null;

        SLEDValue ledConfiguration;
        if (animation.Frames[currentFrameIndex].Values.ContainsKey(selectedLED.Value))
        {
            ledConfiguration = animation.Frames[currentFrameIndex].Values[selectedLED.Value];
        }
        else
        {
            ledConfiguration = new();
            ledConfiguration.InterpolationType = EInterpolationType.NONE;
            ledConfiguration.Led = selectedLED.Value;
            ledConfiguration.Brightness = 0;
        }

        return ledConfiguration;
    }

    private void UpdateCurrentLEDConfiguration(SLEDValue configuration)
    {
        EAddressable? selectedLED = GetSelectedAddressableLED();
        if (selectedLED is null)
            return;

        if (animation.Frames[currentFrameIndex].Values.ContainsKey(selectedLED.Value))
            animation.Frames[currentFrameIndex].Values[selectedLED.Value] = configuration;
        else
            animation.Frames[currentFrameIndex].Values.Add(selectedLED.Value, configuration);
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

        //Load configuration for newly selected LED.
        if (GetCurrentLEDConfiguration() is not SLEDValue ledConfiguration)
            return;

        brightnessInputControlSyncer.SetValue(ledConfiguration.Brightness * SLEDValue.MAX_BRIGHTNESS);
        InterpolationPicker.SelectedIndex = (int)ledConfiguration.InterpolationType;
    }

    private double? Brightness_ValueChanged(double newValue, object? sender)
    {
        hasUnsavedChanges = true;
        double roundedValue = Math.Round(newValue, 1);

        //Update configuration.
        if (GetCurrentLEDConfiguration() is not SLEDValue ledConfiguration)
            return roundedValue;
        //ledConfiguration.Brightness = (float)newValue / SLEDValue.MAX_BRIGHTNESS;
        ledConfiguration.Brightness = (float)newValue * 0.01f; //Multiplication is faster than division.
        UpdateCurrentLEDConfiguration(ledConfiguration);

        //Update live preview.
        GlyphPreview.UpdatePreview(ledConfiguration.Led, newValue);

        //Update physical LED.
        if (API.Running)
            _ = API.Instance.SetBrightness(ledConfiguration.Led, ledConfiguration.Brightness);

        return roundedValue;
    }

    private double? TransitionTime_ValueChanged(double newValue, object? sender)
    {
        hasUnsavedChanges = true;

        //Update configuration.
        SFrame frame = animation.Frames[currentFrameIndex];
        frame.TransitionTime = Math.Clamp((float)newValue, SFrame.MIN_TRANSITION_TIME, SFrame.MAX_TRANSITION_TIME);
        animation.Frames[currentFrameIndex] = frame;

        return Math.Round(frame.TransitionTime, 1);
    }

    private double? Duration_ValueChanged(double newValue, object? sender)
    {
        hasUnsavedChanges = true;

        //Update configuration.
        SFrame frame = animation.Frames[currentFrameIndex];
        frame.Duration = Math.Clamp((float)newValue, SFrame.MIN_DURATION, SFrame.MAX_DURATION);
        animation.Frames[currentFrameIndex] = frame;

        return Math.Round(frame.Duration, 1);
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!await Storage.SaveAnimation(animation))
                throw new IOException();
            hasUnsavedChanges = false;
            canDiscardChanges = false;

            await CommunityToolkit.Maui.Alerts.Toast.Make("Animation saved.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
        }
        catch
        {
            await CommunityToolkit.Maui.Alerts.Toast.Make("Failed to save animation.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
        }
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
        if (!hasUnsavedChanges || canDiscardChanges)
            return false;

        canDiscardChanges = true;

        CommunityToolkit.Maui.Alerts.Toast.Make("Unsaved changes.\nPress again to discard changes.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();

        return true;
    }
    #endregion
}
