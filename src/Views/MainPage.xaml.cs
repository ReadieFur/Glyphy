namespace Glyphy.Views;

public partial class MainPage : ContentPage
{
    private SemaphoreSlim updateLock = new(1);

    public MainPage()
	{
		InitializeComponent();
		VersionNumber.Text = AppInfo.VersionString;
        ToggleControls(false);
	}

    private void ToggleControls(bool enabled)
    {
        LEDPicker.IsEnabled = enabled;
        LEDSlider.IsEnabled = enabled;
        LEDValue.IsEnabled = enabled;
    }

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

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        ToggleControls(LED.API.Running);

#if ANDROID
        Android_ContentPage_Loaded(sender, e);
#endif
    }

    private async void LEDPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!updateLock.Wait(0))
            return;

        LED.EAddressable? selectedLED = GetSelectedAddressableLED();
        if (selectedLED is null)
            return;

        //Get the current brightness value for the selected LED (see LED.API.GetBrightness).
        UpdateInputs((int)await LED.API.Instance.GetBrightness(selectedLED.Value));

        updateLock.Release();
    }

    private async void LEDSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (!updateLock.Wait(0))
            return;

        //TODO: Clamp this first and store the brightness at the class level.
        await UpdateSelectedLED((uint)e.NewValue);
        UpdateInputs((int)e.NewValue);

        updateLock.Release();
    }

    private async void LEDValue_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!updateLock.Wait(0))
            return;

        double value;
        if (!double.TryParse(e.NewTextValue, out value))
            if (!double.TryParse(e.OldTextValue, out value))
                value = 0;

        await UpdateSelectedLED((uint)value);
        UpdateInputs((int)value);

        updateLock.Release();
    }

    private async Task UpdateSelectedLED(uint brightness)
    {
        LED.EAddressable? selectedAddressableLED = GetSelectedAddressableLED();
        if (selectedAddressableLED is null)
            return;

        await LED.API.Instance.SetBrightness(selectedAddressableLED.Value, brightness);

        switch (selectedAddressableLED)
        {
            //TODO: Update the UI to reflect the current state of the LED.
        }
    }

    private void UpdateInputs(int value)
    {
        uint clampedValue = LED.API.Instance.ClampBrightness(value);

        LEDSlider.Value = clampedValue;
        LEDValue.Text = clampedValue.ToString();
    }
}
