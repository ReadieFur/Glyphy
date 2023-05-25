using Glyphy.Configuration;
using Glyphy.Misc;
using Glyphy.Views;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace Glyphy.Controls;

//TODO: Change the code structure so that this class handles all of the actions for the animations related to the glyph entry and then have the MainPage only handle the addition and removal of the glyph entries.
//TODO: When files are loaded, if they fail assume they have been deleted and remove this entry from the UI but don't attempt to delete the file.
public partial class GlyphEntry : ContentView
{
    /*public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(GlyphEntry), "Untitled");
	public string Name { get => (string)GetValue(NameProperty); set => SetValue(NameProperty, value); }*/

    public Guid ID { get; private set; }

    public event EventHandler? OnDeleted;

    private GlyphConfigurator? glyphConfigurator = null;
    private DateTime lastDeleteClick = DateTime.MinValue;
	private CommunityToolkit.Maui.Core.IToast? deletionToast = null;

    [Obsolete("Parameterless constructor only exists for the XAML.", true)]
    private GlyphEntry()
	{
		InitializeComponent();
	}

    public GlyphEntry(Guid ID)
    {
        this.ID = ID;

        InitializeComponent();

        //I had thought about storing the animation in this class however I decided that I would prefer to re-load the animation each time it is needed, a slight performance hit but less memory usage and external changes can be made.
        if (Storage.LoadAnimation(ID).Result is not SAnimation)
            throw new Exception("Failed to load animation.");
    }

    private void ActionButton_Tapped(object sender, TappedEventArgs e)
	{
        //TODO: Have this change to a stop button when playing.
    }

    private void EditButton_Tapped(object sender, TappedEventArgs e)
	{
        //TODO: Disable the controls while an editor is active.
        Task.Run(() =>
        {
            try { glyphConfigurator = new(ID); }
            catch
            {
                _ = CommunityToolkit.Maui.Alerts.Toast.Make("Failed to load animation.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
                return;
            }

            glyphConfigurator.Disappearing += GlyphConfigurator_Disappearing;

            Dispatcher.Dispatch(() => _ = Navigation.PushAsync(glyphConfigurator, true));
        });
    }

    private void GlyphConfigurator_Disappearing(object? sender, EventArgs e)
    {
        //glyphConfigurator won't be null here.
        NameLabel.Text = glyphConfigurator!.animation.Name;
        glyphConfigurator.Dispose();
        glyphConfigurator = null;

        if (Navigation.NavigationStack.Count != 0 && Navigation.NavigationStack[Navigation.NavigationStack.Count - 1] is IThemeChangeHandler themeChangeHandler)
            themeChangeHandler.RequestedThemeChanged(Application.Current!.RequestedTheme == AppTheme.Dark);
    }

    private void DeleteButton_Tapped(object sender, TappedEventArgs e)
	{
		if (lastDeleteClick < DateTime.Now - TimeSpan.FromSeconds(5))
		{
            lastDeleteClick = DateTime.Now;
			deletionToast = CommunityToolkit.Maui.Alerts.Toast.Make("Click again to confirm deletion.", CommunityToolkit.Maui.Core.ToastDuration.Short);
            deletionToast.Show();
			return;
		}

        Storage.DeleteAnimation(ID);

        if (deletionToast is not null)
        {
            deletionToast.Dismiss();
            deletionToast = null;
        }

        OnDeleted?.Invoke(this, e);
    }
}
