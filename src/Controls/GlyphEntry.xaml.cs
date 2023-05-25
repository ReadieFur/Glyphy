using Glyphy.Animation;
using Glyphy.Configuration;
using Glyphy.Misc;
using Glyphy.Views;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Glyphy.Controls;

//TODO: Change the code structure so that this class handles all of the actions for the animations related to the glyph entry and then have the MainPage only handle the addition and removal of the glyph entries.
//TODO: When files are loaded, if they fail assume they have been deleted and remove this entry from the UI but don't attempt to delete the file.
public partial class GlyphEntry : ContentView, IDisposable, IThemeChangeHandler
{
    /*public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(GlyphEntry), "Untitled");
	public string Name { get => (string)GetValue(NameProperty); set => SetValue(NameProperty, value); }*/

    public Guid AnimationID { get; private set; }

    public event EventHandler? OnDeleted;

    private GlyphConfigurator? glyphConfigurator = null;
    private DateTime lastDeleteClick = DateTime.MinValue;
	private CommunityToolkit.Maui.Core.IToast? deletionToast = null;

    [Obsolete("Parameterless constructor only exists for the XAML preview.", true)]
    private GlyphEntry()
	{
		InitializeComponent();
	}

    public GlyphEntry(Guid AnimationID)
    {
        this.AnimationID = AnimationID;

        InitializeComponent();

        //I had thought about storing the animation in this class however I decided that I would prefer to re-load the animation each time it is needed, a slight performance hit but less memory usage and external changes can be made.
        if (Storage.LoadAnimation(AnimationID).Result is not SAnimation animation)
            throw new Exception("Failed to load animation.");

        SetNameLabel(animation.Name);

        AnimationRunner.OnStateChanged += AnimationRunner_OnStateChanged;
    }

    public void Dispose()
    {
        AnimationRunner.OnStateChanged -= AnimationRunner_OnStateChanged;
    }

    private void AnimationRunner_OnStateChanged(SAnimation? newAnimation)
    {
        Dispatcher.Dispatch(() => ToggleControls(true, true, true));

        if (newAnimation is SAnimation && newAnimation.Value.Id == AnimationID)
        {
            //Set the activity button to a pause button.
            Dispatcher.Dispatch(() =>
            {
                ActionIcon.Text = "\uf04c";
                ActionLabel.Text = "Pause";
            });
        }
        else
        {
            //Set the activity button to a play button.
            Dispatcher.Dispatch(() =>
            {
                ActionIcon.Text = "\uf04b";
                ActionLabel.Text = "Play";
            });
        }
    }

    private void ActionButton_Tapped(object sender, TappedEventArgs e)
	{
        Task.Run(async () =>
        {
            SAnimation? previousAnimation = AnimationRunner.ActiveAnimation;

            //We will always stop a task here as we will either be starting a new animation or stopping the current one, both which require a stopping of the previous animation.
            CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(2500));
            Task stopResult = AnimationRunner.StopAnimation(cancellationTokenSource.Token);
            await stopResult;

            if (stopResult.IsCanceled || stopResult.IsFaulted)
            {
                _ = CommunityToolkit.Maui.Alerts.Toast.Make("Failed to stop current animation.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                return;
            }

            //If the previous animation was the same as the current animation then we don't need to start a new animation.
            if (previousAnimation is SAnimation && previousAnimation.Value.Id == AnimationID)
                return;

            //Disable the activity controls, the AnimationRunner_OnStateChanged callback will re-enable them (unless we fail to start the animation).
            Dispatcher.Dispatch(() => ToggleControls(false, false, false));

            //Start the animation.
            if (await Storage.LoadAnimation(AnimationID) is not SAnimation animation)
            {
                _ = CommunityToolkit.Maui.Alerts.Toast.Make("Failed to load animation.", CommunityToolkit.Maui.Core.ToastDuration.Short);
                return;
            }

            cancellationTokenSource = new();
            cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(2500));
            Task startResult = AnimationRunner.StartAnimation(animation, cancellationTokenSource.Token);
            await startResult;

            if (startResult.IsCanceled || startResult.IsFaulted)
            {
                _ = CommunityToolkit.Maui.Alerts.Toast.Make("Failed to start animation.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                Dispatcher.Dispatch(() => ToggleControls(true, true, true));
            }
        });
    }

    private void EditButton_Tapped(object sender, TappedEventArgs e)
	{
        //TODO: Disable the controls while an editor is active.
        Task.Run(() =>
        {
            try { glyphConfigurator = new(AnimationID); }
            catch
            {
                _ = CommunityToolkit.Maui.Alerts.Toast.Make("Failed to load animation.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                return;
            }

            glyphConfigurator.Disappearing += GlyphConfigurator_Disappearing;

            Dispatcher.Dispatch(() => _ = Navigation.PushAsync(glyphConfigurator, true));
        });
    }

    private void GlyphConfigurator_Disappearing(object? sender, EventArgs e)
    {
        //glyphConfigurator won't be null here.
        SetNameLabel(glyphConfigurator!.Animation.Name);
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

        Storage.DeleteAnimation(AnimationID);

        if (deletionToast is not null)
        {
            deletionToast.Dismiss();
            deletionToast = null;
        }

        OnDeleted?.Invoke(this, e);
    }

    private void SetNameLabel(string text) =>
        NameLabel.Text = text.Length > 20 - 3 ? text.Substring(0, 20 - 3) + "..." : text;

    public void ToggleControls(bool actionButtonEnabled, bool editButtonEnabled, bool deleteButtonEnabled)
    {
        Color foregroundColour = Application.Current!.RequestedTheme == AppTheme.Dark ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#000000");
        Color disabledColour = Color.FromArgb("#808080");

        if (actionButtonEnabled)
        {
            ActionContainer.IsEnabled = true;
            ActionIcon.TextColor = foregroundColour;
            ActionLabel.TextColor = foregroundColour;
        }
        else
        {
            ActionContainer.IsEnabled = true;
            ActionIcon.TextColor = disabledColour;
            ActionLabel.TextColor = disabledColour;
        }

        if (editButtonEnabled)
        {
            EditContainer.IsEnabled = editButtonEnabled;
            EditIcon.TextColor = foregroundColour;
            EditLabel.TextColor = foregroundColour;
        }
        else
        {
            EditContainer.IsEnabled = editButtonEnabled;
            EditIcon.TextColor = disabledColour;
            EditLabel.TextColor = disabledColour;
        }

        if (deleteButtonEnabled)
        {
            DeleteContainer.IsEnabled = deleteButtonEnabled;
            DeleteIcon.TextColor = foregroundColour;
            DeleteLabel.TextColor = foregroundColour;
        }
        else
        {
            DeleteContainer.IsEnabled = deleteButtonEnabled;
            DeleteIcon.TextColor = disabledColour;
            DeleteLabel.TextColor = disabledColour;
        }
    }

    public void RequestedThemeChanged(bool isDark)
    {
        //We can call the ToggleControls method to refresh the controls colours.
        ToggleControls(ActionContainer.IsEnabled, EditContainer.IsEnabled, DeleteContainer.IsEnabled);
    }
}
