using Glyphy.Animation;
using Glyphy.Configuration;
using Glyphy.LED;
using Glyphy.Misc;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Glyphy.Views;

public partial class GlyphConfigurator : ContentPage, IDisposable
{
    #region Properties
    private SAnimation _animation;
    private InputControlSyncer<double> brightnessInputControlSyncer;
    private InputControlSyncer<double> transitionTimeInputControlSyncer;
    private InputControlSyncer<double> durationInputControlSyncer;

    public SAnimation Animation => _animation;
    //TODO: Have the save icon go from inactive to active when there are unsaved changes.
    private bool hasUnsavedChanges = false;
    //TODO: Have the back icon go red when the ability to discard changes is available.
    private bool canDiscardChanges = false;
    private int currentFrameIndex = 0;
    #endregion

    #region Constructors
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public GlyphConfigurator()
    {
        //Set to true for new animations.
        hasUnsavedChanges = true;
        _animation = SAnimation.CreateNewAnimation();

        SharedConstructor();
	}

    public GlyphConfigurator(Guid id)
    {
        //TODO: Move this out of the constructor as it contains long running code (async methods).
        SAnimation? animation = Storage.LoadAnimation(id).Result;
        if (animation is null)
            throw new IOException("Failed to load animation.");
        _animation = animation.Value;

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

        AnimationNameEntry.Text = _animation.Name;

        TransitionTimeSlider.Minimum = SFrame.MIN_TRANSITION_TIME;
        TransitionTimeSlider.Maximum = SFrame.MAX_TRANSITION_TIME;

        DurationSlider.Minimum = SFrame.MIN_DURATION;
        DurationSlider.Maximum = SFrame.MAX_DURATION;

        #region Inputs
        AnimationNameEntry.TextChanged += AnimationNameEntry_TextChanged;
        LEDPicker.SelectedIndexChanged += LEDPicker_SelectedIndexChanged;
        InterpolationPicker.SelectedIndexChanged += InterpolationPicker_SelectedIndexChanged;

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
        if (_animation.Frames[currentFrameIndex].Values.ContainsKey(selectedLED.Value))
        {
            ledConfiguration = _animation.Frames[currentFrameIndex].Values[selectedLED.Value];
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

        SFrame frame = _animation.Frames[currentFrameIndex];
        if (_animation.Frames[currentFrameIndex].Values.ContainsKey(selectedLED.Value))
            _animation.Frames[currentFrameIndex].Values[selectedLED.Value] = configuration;
        else
            _animation.Frames[currentFrameIndex].Values.Add(selectedLED.Value, configuration);
        _animation.Frames[currentFrameIndex] = frame;
    }

    //TODO: Use the animator to transition to the new frame.
    private void LoadFrame(uint frameIndex)
    {
        if (frameIndex >= _animation.Frames.Count)
            throw new IndexOutOfRangeException("Frame index is out of range.");
        currentFrameIndex = (int)frameIndex;

        transitionTimeInputControlSyncer.SetValue(Math.Round(_animation.Frames[currentFrameIndex].TransitionTime, 1));
        durationInputControlSyncer.SetValue(Math.Round(_animation.Frames[currentFrameIndex].Duration, 1));

        Task.Run(() =>
        {
            foreach (EAddressable led in Enum.GetValues(typeof(EAddressable)))
            {
                if (_animation.Frames[currentFrameIndex].Values.ContainsKey(led))
                {
                    if (API.Running)
                        _ = API.Instance.SetBrightness(led, _animation.Frames[currentFrameIndex].Values[led].Brightness);
                    GlyphPreview.UpdatePreview(led, _animation.Frames[currentFrameIndex].Values[led].Brightness);
                }
                else
                {
                    if (API.Running)
                        _ = API.Instance.SetBrightness(led, 0);
                    GlyphPreview.UpdatePreview(led, 0);
                }
            }
        });

        //TODO: Update the UI to reflect the new frame.
        if (_animation.Frames[currentFrameIndex].Values.Count == 0)
            return;

        KeyValuePair<EAddressable, SLEDValue> firstLED = _animation.Frames[currentFrameIndex].Values.First();
        LEDPicker.SelectedIndex = (int)firstLED.Key;
        brightnessInputControlSyncer.SetValue(Math.Round(firstLED.Value.Brightness * 100, 1));
        InterpolationPicker.SelectedIndex = (int)firstLED.Value.InterpolationType;
    }
    #endregion

    #region Event handlers
    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        bool hasUnsavedChanges = this.hasUnsavedChanges;

        ToggleControls(API.Running);

        LoadFrame(0);

#if ANDROID
        Android_ContentPage_Loaded(sender, e);
#endif

        this.hasUnsavedChanges = hasUnsavedChanges;
    }

    private void AnimationNameEntry_TextChanged(object? sender, TextChangedEventArgs e)
    {
        hasUnsavedChanges = true;

        //Update configuration.
        _animation.Name = AnimationNameEntry.Text;
    }

    private void LEDPicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        hasUnsavedChanges = true;

        //Load configuration for newly selected LED.
        if (GetCurrentLEDConfiguration() is not SLEDValue ledConfiguration)
            return;

        brightnessInputControlSyncer.SetValue(ledConfiguration.Brightness * 100);
        InterpolationPicker.SelectedIndex = (int)ledConfiguration.InterpolationType;
    }

    private void InterpolationPicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        hasUnsavedChanges = true;

        //Update configuration.
        if (GetCurrentLEDConfiguration() is not SLEDValue ledConfiguration)
            return;
        ledConfiguration.InterpolationType = (EInterpolationType)InterpolationPicker.SelectedIndex;
        UpdateCurrentLEDConfiguration(ledConfiguration);
    }

    private double? Brightness_ValueChanged(double newValue, object? sender)
    {
        hasUnsavedChanges = true;

        double roundedValue = Math.Round(newValue, 1);

        //Update configuration.
        if (GetCurrentLEDConfiguration() is not SLEDValue ledConfiguration)
            return roundedValue;
        ledConfiguration.Brightness = (float)newValue * 0.01f; //Multiplication is faster than division.
        UpdateCurrentLEDConfiguration(ledConfiguration);

        //Update live preview.
        GlyphPreview.UpdatePreview(ledConfiguration.Led, ledConfiguration.Brightness);

        //Update physical LED.
        if (API.Running)
            _ = API.Instance.SetBrightness(ledConfiguration.Led, ledConfiguration.Brightness);

        return roundedValue;
    }

    private double? TransitionTime_ValueChanged(double newValue, object? sender)
    {
        hasUnsavedChanges = true;

        //Update configuration.
        SFrame frame = _animation.Frames[currentFrameIndex];
        frame.TransitionTime = Math.Clamp((float)newValue, SFrame.MIN_TRANSITION_TIME, SFrame.MAX_TRANSITION_TIME);
        _animation.Frames[currentFrameIndex] = frame;

        return Math.Round(frame.TransitionTime, 1);
    }

    private double? Duration_ValueChanged(double newValue, object? sender)
    {
        hasUnsavedChanges = true;

        //Update configuration.
        SFrame frame = _animation.Frames[currentFrameIndex];
        frame.Duration = Math.Clamp((float)newValue, SFrame.MIN_DURATION, SFrame.MAX_DURATION);
        _animation.Frames[currentFrameIndex] = frame;

        return Math.Round(frame.Duration, 1);
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            _animation.Normalize();
            if (!await Storage.SaveAnimation(_animation))
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
        {
            Task.Run(() =>
            {
                if (!API.Running)
                    return;

                //Reset all LEDs to their default state.
                foreach (EAddressable led in Enum.GetValues(typeof(EAddressable)))
                    _ = API.Instance.SetBrightness(led, 0);
            });

            return false;
        }

        canDiscardChanges = true;

        CommunityToolkit.Maui.Alerts.Toast.Make("Unsaved changes.\nPress again to discard changes.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();

        return true;
    }
    #endregion
}
