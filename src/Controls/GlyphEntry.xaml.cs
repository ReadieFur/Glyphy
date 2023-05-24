using Microsoft.Maui.Controls;
using System;

namespace Glyphy.Controls;

//TODO: Change the code structure so that this class handles all of the actions for the animations related to the glyph entry and then have the MainPage only handle the addition and removal of the glyph entries.
public partial class GlyphEntry : ContentView
{
	/*public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(GlyphEntry), "Untitled");
	public string Name { get => (string)GetValue(NameProperty); set => SetValue(NameProperty, value); }*/

	public Guid ID { init; get; }

	public string Name
	{
		get => NameLabel.Text;
		set => NameLabel.Text = value;
	}

    public event EventHandler? OnActionButtonTapped;
    public event EventHandler? OnEditButtonTapped;
    public event EventHandler? OnDeleteButtonTapped;

	private DateTime lastDeleteClick = DateTime.MinValue;
	private CommunityToolkit.Maui.Core.IToast? deletionToast = null;

    public GlyphEntry()
	{
		InitializeComponent();
	}

    //TODO: Have this change to a stop button when playing.
    private void ActionButton_Tapped(object sender, TappedEventArgs e) =>
		OnActionButtonTapped?.Invoke(this, e);

    private void EditButton_Tapped(object sender, TappedEventArgs e) =>
		OnEditButtonTapped?.Invoke(this, e);

    private void DeleteButton_Tapped(object sender, TappedEventArgs e)
	{
		if (lastDeleteClick < DateTime.Now - TimeSpan.FromSeconds(5))
		{
            lastDeleteClick = DateTime.Now;
			deletionToast = CommunityToolkit.Maui.Alerts.Toast.Make("Click again to confirm deletion.", CommunityToolkit.Maui.Core.ToastDuration.Short);
            deletionToast.Show();
			return;
		}

        OnDeleteButtonTapped?.Invoke(this, e);
        if (deletionToast is not null)
        {
            deletionToast.Dismiss();
            deletionToast = null;
        }
    }
}
