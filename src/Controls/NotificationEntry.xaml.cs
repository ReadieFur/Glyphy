namespace Glyphy.Controls;

public partial class NotificationEntry : ContentView
{
    public readonly string PackageName;

    [Obsolete("Parameterless constructor only exists for the XAML preview.", true)]
    public NotificationEntry()
    {
        InitializeComponent();
    }

    public NotificationEntry(string packageName, ImageSource? appIcon, string appLabel)
    {
        InitializeComponent();

        PackageName = packageName;

        AppIcon.Source = appIcon;
        NameLabel.Text = appLabel;

        EnabledSwitch.IsToggled = false;
        GlyphPicker.IsEnabled = false;
        //GlyphPicker.SelectedIndex = glyphs.Keys.ToList().IndexOf(Glyphy.Resources.Presets.Glyphs.OFF.Id);
        GlyphPicker.SelectedIndex = -1;

        EnabledSwitch.IsEnabled = true;
    }
}
