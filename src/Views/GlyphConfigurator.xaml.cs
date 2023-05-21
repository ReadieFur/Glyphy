using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;

namespace Glyphy.Views;

public partial class GlyphConfigurator : ContentPage, IDisposable
{
	public GlyphConfigurator()
	{
        SharedConstructor();
	}

    public GlyphConfigurator(Guid id)
    {
        SharedConstructor();
    }

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

        //These get added in order of the enum so we can use the index to get the enum value (though to be safe I will re-format them).
        foreach (string key in Enum.GetNames<LED.EAddressable>())
            LEDPicker.Items.Add(EnumKeyToFormattedString(key));
        LEDPicker.SelectedIndex = 0;

        foreach (string key in Enum.GetNames<Configuration.EInterpolationType>())
            InterpolationPicker.Items.Add(EnumKeyToFormattedString(key));
        InterpolationPicker.SelectedIndex = 0;
    }

    private void ToggleControls(bool enabled)
    {
        ControlsGroup.IsEnabled = enabled;
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        ToggleControls(LED.API.Running);

#if ANDROID
        Android_ContentPage_Loaded(sender, e);
#endif
    }
}
