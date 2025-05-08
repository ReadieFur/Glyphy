using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Glyphy.Animation;
using Glyphy.Configuration;
using Glyphy.LED;
using Glyphy.Misc;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Helpers = Glyphy.Misc.Helpers;

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
        _animation = new();
        //Add a default frame.
        _animation.Frames.Add(new());

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
        MainApplication.OnResume -= MainApplication_OnResume;
    }

    private void SharedConstructor()
    {
        InitializeComponent();

        ToggleControls(false);

        for (uint i = 0; i < _animation.Frames.Count; i++)
            FrameList.Add(CreateFrameButton(i));

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

        foreach (Button frameButton in FrameList)
            frameButton.SetAppTheme(Button.BackgroundColorProperty, Color.FromArgb("#F2F2F2"), Color.FromArgb("#0D0D0D"));
        ((Button)FrameList[(int)frameIndex]).BackgroundColor = Colors.Transparent;

        SLEDValue ledValue;
        if (_animation.Frames[currentFrameIndex].Values.Count > 0)
        {
            KeyValuePair<EAddressable, SLEDValue> firstLED = _animation.Frames[currentFrameIndex].Values.First();
            ledValue = firstLED.Value;
            LEDPicker.SelectedIndex = (int)firstLED.Key;
        }
        else
        {
            ledValue = new();
            LEDPicker.SelectedIndex = 0;
        }
        brightnessInputControlSyncer.SetValue(Math.Round(ledValue.Brightness * 100, 1));
        InterpolationPicker.SelectedIndex = (int)ledValue.InterpolationType;
    }

    private Button CreateFrameButton(uint index)
    {
        Button frameButton = new();
        frameButton.Text = $"{index + 1}";
        frameButton.Clicked += (_, _) => LoadFrame(uint.Parse(frameButton.Text) - 1);
        frameButton.SetAppTheme(Button.TextColorProperty, Colors.Black, Colors.White);
        frameButton.SetAppTheme(Button.BackgroundColorProperty, Color.FromArgb("#F2F2F2"), Color.FromArgb("#0D0D0D"));
        return frameButton;
    }
    #endregion

    #region Event handlers
    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        bool hasUnsavedChanges = this.hasUnsavedChanges;

        ToggleControls(API.Running);

        LoadFrame(0);

        Header.Padding = new(Header.Padding.Left, /*Header.Padding.Top + */Helpers.StatusBarHeight, Header.Padding.Right, Header.Padding.Bottom);
        Padding = new(Padding.Left, Padding.Top, Padding.Right, /*Padding.Bottom + */Helpers.NavigationBarHeight);

        //Potential race condition here where the check runs before the API starts.
        MainApplication.OnResume += MainApplication_OnResume;

        this.hasUnsavedChanges = hasUnsavedChanges;
    }

    private void MainApplication_OnResume(Android.App.Activity activity) =>
        ToggleControls(API.Running);

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

        double clampedValue = Math.Clamp(newValue, 0, 100);
        double roundedValue = Math.Round(clampedValue, 1);

        //Update configuration.
        if (GetCurrentLEDConfiguration() is not SLEDValue ledConfiguration)
            return roundedValue;
        ledConfiguration.Brightness = Misc.Helpers.ConvertNumberRange((float)clampedValue, 0, 100, SLEDValue.MIN_BRIGHTNESS, SLEDValue.MAX_BRIGHTNESS);
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

        double clampedValue = Math.Clamp(newValue, SFrame.MIN_TRANSITION_TIME, SFrame.MAX_TRANSITION_TIME);
        double roundedValue = Math.Round(clampedValue, 2);

        //Update configuration.
        SFrame frame = _animation.Frames[currentFrameIndex];
        frame.TransitionTime = (float)clampedValue;
        _animation.Frames[currentFrameIndex] = frame;

        return roundedValue;
    }

    private double? Duration_ValueChanged(double newValue, object? sender)
    {
        hasUnsavedChanges = true;

        double clampedValue = Math.Clamp(newValue, SFrame.MIN_DURATION, SFrame.MAX_DURATION);
        double roundedValue = Math.Round(clampedValue, 2);

        //Update configuration.
        SFrame frame = _animation.Frames[currentFrameIndex];
        frame.Duration = (float)clampedValue;
        _animation.Frames[currentFrameIndex] = frame;

        return roundedValue;
    }

    /// <summary>
    /// Deletes the currently selected frame.
    /// </summary>
    private void DeleteFrameButton_Clicked(object sender, EventArgs e)
    {
        hasUnsavedChanges = true;

        _animation.Frames.RemoveAt(currentFrameIndex);
        FrameList.RemoveAt(currentFrameIndex);

        //Shift all of the UI button values on the right of the deleted frame down by one.
        for (int i = currentFrameIndex; i < FrameList.Count; i++)
            ((Button)FrameList[i]).Text = $"{i + 1}";

        //Modulo cannot be used here in-case we are deleting the last frame (division by zero).
        int newActiveFrameIndex = currentFrameIndex - 1;

        //If the new active frame index is less than zero, and there are no more frames, create a new empty frame.
        if (newActiveFrameIndex < 0 && _animation.Frames.Count == 0)
        {
            _animation.Frames.Add(new());
            FrameList.Add(CreateFrameButton(0));
            newActiveFrameIndex = 0;
        }
        //Otherwise, if the new active frame index is less than zero, set it to zero.
        else if (newActiveFrameIndex < 0)
        {
            newActiveFrameIndex = 0;
        }

        LoadFrame((uint)newActiveFrameIndex);
    }

    /// <summary>
    /// Adds a new frame to the right of the currently selected frame.
    /// </summary>
    private void AddFrameButton_Clicked(object sender, EventArgs e)
    {
        hasUnsavedChanges = true;

        _animation.Frames.Insert(currentFrameIndex + 1, new());
        FrameList.Insert(currentFrameIndex + 1, CreateFrameButton((uint)currentFrameIndex + 1));

        //Shift all of the UI button values on the right of the new frame up by one.
        for (int i = currentFrameIndex + 1; i < FrameList.Count; i++)
            ((Button)FrameList[i]).Text = $"{i + 1}";

        LoadFrame((uint)currentFrameIndex + 1);
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

            await Toast.Make("Animation saved.", ToastDuration.Short).Show();
        }
        catch
        {
            await Toast.Make("Failed to save animation.", ToastDuration.Long).Show();
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

        Toast.Make("Unsaved changes.\nPress again to discard changes.", ToastDuration.Long).Show();

        return true;
    }
    #endregion
}
