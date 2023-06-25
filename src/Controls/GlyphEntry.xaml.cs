using CommunityToolkit.Maui.Alerts;
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
public partial class GlyphEntry : ContentView, IDisposable
{
    /*public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(GlyphEntry), "Untitled");
	public string Name { get => (string)GetValue(NameProperty); set => SetValue(NameProperty, value); }*/

    public Guid AnimationID { get; private set; }
    public bool IsReadonly { get; private set; }

    public event EventHandler? OnDeleted;

    private Task? tappedActionTask = null;
    private GlyphConfigurator? glyphConfigurator = null;
    private DateTime lastDeleteClick = DateTime.MinValue;
	private CommunityToolkit.Maui.Core.IToast? deletionToast = null;

    [Obsolete("Parameterless constructor only exists for the XAML preview.", true)]
    private GlyphEntry()
	{
		InitializeComponent();
	}

    public GlyphEntry(Guid AnimationID, bool isReadonly = false)
    {
        this.AnimationID = AnimationID;
        this.IsReadonly = isReadonly;

        InitializeComponent();

        //I had thought about storing the animation in this class however I decided that I would prefer to re-load the animation each time it is needed, a slight performance hit but less memory usage and external changes can be made.
        if (Storage.LoadAnimation(AnimationID).Result is not SAnimation animation)
            throw new Exception("Failed to load animation.");

        SetNameLabel(animation.Name);

        bool hideControls = isReadonly || AnimationRunner.IsRunning;
        ToggleControls(true, hideControls, hideControls);

        AnimationRunner.OnStateChanged += AnimationRunner_OnStateChanged;

    }

    public void Dispose()
    {
        AnimationRunner.OnStateChanged -= AnimationRunner_OnStateChanged;
    }

    private void AnimationRunner_OnStateChanged(SAnimation? newAnimation)
    {
        if (newAnimation is SAnimation && newAnimation.Value.Id == AnimationID)
        {
            //Set the activity button to a stop button.
            Dispatcher.Dispatch(() =>
            {
                ToggleControls(true, false, false);
                ActionIcon.Text = "\uf04d";
                ActionLabel.Text = "Stop";
            });
        }
        else
        {
            //Set the activity button to a play button.
            Dispatcher.Dispatch(() =>
            {
                ToggleControls(true, true, true);
                ActionIcon.Text = "\uf04b";
                ActionLabel.Text = "Play";
            });
        }
    }

    private void ActionButton_Tapped(object sender, TappedEventArgs e)
	{
        //We don't need to lock here as this runs on the main thread and is only ever invoked from the UI.
        if (tappedActionTask is not null)
            return;

        //TODO: I think this is the point where the animation runner can freeze up but its difficult to figure out what it is that is breaking here.
        tappedActionTask = Task.Run(async () =>
        {
            SAnimation? previousAnimation = AnimationRunner.ActiveAnimation;

            if (AnimationRunner.IsRunning)
            {
                try
                {
                    CancellationTokenSource cancellationTokenSource = new();
                    cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(2500));
                    await AnimationRunner.StopAnimation(cancellationTokenSource.Token);
                }
                catch
                {
                    Dispatcher.Dispatch(() => Toast.Make("Failed to stop current animation.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show());
                    return;
                }
            }
            
            //If the previous animation was the same as the current animation then we don't need to start a new animation.
            if (previousAnimation is SAnimation && previousAnimation.Value.Id == AnimationID)
                return;

            //Disable the activity controls, the AnimationRunner_OnStateChanged callback will re-enable them (unless we fail to start the animation).
            Dispatcher.Dispatch(() => ToggleControls(false, false, false));

            //Start the animation.
            if (await Storage.LoadAnimation(AnimationID) is not SAnimation animation)
            {
                Dispatcher.Dispatch(() => Toast.Make("Failed to load animation.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show());
                return;
            }

            Task startResult;
            try
            {
                CancellationTokenSource cancellationTokenSource = new();
                cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(2500));
                startResult = AnimationRunner.StartAnimation(animation, cancellationTokenSource.Token);
                await startResult;
            }
            catch
            {
                Dispatcher.Dispatch(() =>
                {
                    Toast.Make("Failed to start animation.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
                    ToggleControls(true, true, true);
                });
            }
        });
        tappedActionTask.ContinueWith((task) => tappedActionTask = null);
    }

    private void EditButton_Tapped(object sender, TappedEventArgs e)
	{
        //TODO: Disable the controls while an editor is active.
        Task.Run(() =>
        {
            try { glyphConfigurator = new(AnimationID); }
            catch
            {
                Dispatcher.Dispatch(() => Toast.Make("Failed to load animation.", CommunityToolkit.Maui.Core.ToastDuration.Short).Show());
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
    }

    private void DeleteButton_Tapped(object sender, TappedEventArgs e)
	{
		if (lastDeleteClick < DateTime.Now - TimeSpan.FromSeconds(5))
		{
            lastDeleteClick = DateTime.Now;
			deletionToast = Toast.Make("Click again to confirm deletion.", CommunityToolkit.Maui.Core.ToastDuration.Short);
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
        if (IsReadonly)
        {
            editButtonEnabled = false;
            deleteButtonEnabled = false;
        }

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
}
