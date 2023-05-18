using Microsoft.Maui.Controls;
using System;
using LED = Glyphy.Platforms.Android.LED;
using Android.Widget;
using Microsoft.Maui.ApplicationModel;
using System.Threading;

namespace Glyphy.Views;

public partial class MainPage : ContentPage
{
    private SemaphoreSlim updateLock = new(1);

    private void ContentPage_Loaded(object sender, System.EventArgs e)
    {
        //Initalize the API.
        if (!LED.API.Instance.Running)
            WrapAPICall(() => _ = LED.API.Instance);
    }

    private void LEDPicker_SelectedIndexChanged(object sender, EventArgs e) => UpdateWrapper(() =>
    {
        LED.EAddressable? selectedLED = GetSelectedAddressableLED();
        if (selectedLED is null)
            return;

        //Get the current brightness value for the selected LED.
        //TODO: <See LED.API.GetBrightness>
        uint brightness = 0;
        WrapAPICall(() => brightness = LED.API.Instance.GetBrightness(selectedLED.Value));

        //Update the UI.
        //In this case the values SHOULD already be clamped but it dosen't hurt here to reclamp them.
        UpdateInputs((int)brightness);
    });

    private void LEDSlider_ValueChanged(object sender, ValueChangedEventArgs e) => UpdateWrapper(() =>
    {
        //TODO: Clamp this first and store the brightness at the class level.
        UpdateSelectedLED((uint)e.NewValue);
        UpdateInputs((int)e.NewValue);
    });

    private void LEDValue_TextChanged(object sender, TextChangedEventArgs e) => UpdateWrapper(() =>
    {
        double value = 0;
        if (!double.TryParse(e.NewTextValue, out value))
            if (!double.TryParse(e.OldTextValue, out value))
                value = 0;

        UpdateSelectedLED((uint)value);
        UpdateInputs((int)value);
    });

    private LED.EAddressable? GetSelectedAddressableLED()
    {
        return LEDPicker.SelectedIndex switch
        {
            0 => LED.EAddressable.CAMERA,
            1 => LED.EAddressable.DIAGONAL,
            2 => LED.EAddressable.RECORDING_LED,
            3 => LED.EAddressable.CENTER_TOP_LEFT,
            4 => LED.EAddressable.CENTER_TOP_RIGHT,
            5 => LED.EAddressable.CENTER_BOTTOM_LEFT,
            6 => LED.EAddressable.CENTER_BOTTOM_RIGHT,
            7 => LED.EAddressable.LINE_1,
            8 => LED.EAddressable.LINE_2,
            9 => LED.EAddressable.LINE_3,
            10 => LED.EAddressable.LINE_4,
            11 => LED.EAddressable.LINE_5,
            12 => LED.EAddressable.LINE_6,
            13 => LED.EAddressable.LINE_7,
            14 => LED.EAddressable.LINE_8,
            15 => LED.EAddressable.DOT,
            _ => null
        };
    }

    private void UpdateWrapper(Action action)
    {
        if (!updateLock.Wait(0))
            return;
        action.Invoke();
        updateLock.Release();
    }

    private void UpdateSelectedLED(uint brightness)
    {
        LED.EAddressable? selectedAddressableLED = GetSelectedAddressableLED();
        if (selectedAddressableLED is null)
            return;

        WrapAPICall(() => LED.API.Instance.SetBrightness(selectedAddressableLED.Value, brightness));

        switch (selectedAddressableLED)
        {
            //TODO: Update the UI to reflect the current state of the LED.
        }
    }

    private void UpdateInputs(int value)
    {
        uint clampedValue = 0;
        WrapAPICall(() => clampedValue = LED.API.Instance.ClampBrightness(value));

        LEDSlider.Value = clampedValue;
        LEDValue.Text = clampedValue.ToString();
    }

    private void WrapAPICall(Action apiCall)
    {
        try
        {
            apiCall.Invoke();
        }
        catch (Exception ex)
        {
            if (ex is PermissionException
                || ex is UnauthorizedAccessException
                || ex is NullReferenceException) //Not sure about catching this last one for all calls. Alternativly I could check if the API is running and initalize based on that but that adds more overhead.
                Toast.MakeText(Android.App.Application.Context, "Superuser permissions required.", ToastLength.Long)?.Show();
            else
                throw;
        }
    }
}
