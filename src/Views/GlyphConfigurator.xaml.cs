using CommunityToolkit.Maui.Alerts;
using Glyphy.Animation;
using Glyphy.Controls.Bezier;
using Glyphy.Glyph;
using Glyphy.Glyph.Indexes;
using Glyphy.Misc;
using Glyphy.Storage;

namespace Glyphy.Views;

public partial class GlyphConfigurator : ContentPage
{
    private readonly GlyphConfiguratorViewModel _viewModel = new();
    private SAnimation _animation = new();
    private bool _hasUnsavedChanges = false, _ignoreUnsavedChanges = false;

	public GlyphConfigurator()
	{
		InitializeComponent();
        Setup();

        OnSelectedInteroplationOptionChanged();
    }

    public GlyphConfigurator(Guid animationId)
    {
        InitializeComponent();
        Setup();

        IsEnabled = false;

        try
        {
            StorageManager.Instance.LoadAnimation(animationId).ContinueWith(async t =>
            {
                if (t.IsFaulted)
                {
                    await Toast.Make(t.Exception?.Message ?? "Failed to load animation.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
                    await Dispatcher.DispatchAsync(async() => await Navigation.PopAsync());
                }

                _animation = t.Result;

                if (_animation.Keyframes.Count > 0)
                {
                    SPhoneIndex index = _animation.Keyframes.Keys.OrderBy(phoneIndex => phoneIndex.Key).First();
                    if (index != _viewModel.SelectedGlyphOption)
                    {
                        //This will trigger the UI to load the rest of the elements for this keyframe, no need to do anything else here.
                        _viewModel.SelectedGlyphOption = index;
                    }
                    else
                    {
                        //Force update as the newly selected glyph option matches the old one.
                        Dispatcher.Dispatch(OnSelectedGlyphChanged);
                    }
                }

                Dispatcher.Dispatch(() => IsEnabled = true);
            }).ConfigureAwait(true);
        }
        catch (FileNotFoundException ex)
        {
            Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Long).Show().Wait();
            Navigation.PopAsync().Wait();
        }
    }

    private void Setup()
    {
        BindingContext = _viewModel;
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        BezierGraph.ViewModel.PropertyChanged += BezierGraph_ViewModel_PropertyChanged;

        GlyphAPI.Instance.WaitForReadyAsync().ContinueWith(t =>
        {
            if (!t.Result)
                throw new InvalidOperationException();

            IEnumerable<string> items;
            switch (GlyphAPI.Instance.PhoneType)
            {
                case EPhoneType.PhoneOne:
                    items = Enum.GetNames<EPhoneOne>();
                    break;
                case EPhoneType.PhoneTwo:
                    items = Enum.GetNames<EPhoneTwo>();
                    break;
                case EPhoneType.PhoneTwoA:
                case EPhoneType.PhoneTwoAPlus:
                    items = Enum.GetNames<EPhoneTwoA>();
                    break;
                case EPhoneType.PhoneThreeA:
                    //case EPhoneType.PhoneThreeAPro:
                    items = Enum.GetNames<EPhoneThreeA>();
                    break;
                default:
                    items = Array.Empty<string>();
                    break;
            }
            items = items.OrderBy(name => name);

            _viewModel.GlyphOptions = [.. items];
            _viewModel.SelectedGlyphOption = new(GlyphAPI.Instance.PhoneType, items.First());
        }).ConfigureAwait(true);
    }

    private async void BackButton_Clicked(object? sender, EventArgs e)
    {
        if (_hasUnsavedChanges && !_ignoreUnsavedChanges)
        {
            await Toast.Make("You have unsaved changes!\nPress back again to discard changes.", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
            _ignoreUnsavedChanges = true;
            return;
        }

        await Navigation.PopAsync();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GlyphConfiguratorViewModel.SelectedInterpolationOption))
            OnSelectedInteroplationOptionChanged();
        else if (e.PropertyName == nameof(GlyphConfiguratorViewModel.Brightness))
            BezierGraph.ViewModel.CurrentY = MathHelpers.ConvertNumberRange(_viewModel.Brightness, 0, 100, -1, 1);
        else if (e.PropertyName == nameof(GlyphConfiguratorViewModel.TimestampValue))
            BezierGraph.ViewModel.CurrentX = MathHelpers.ConvertNumberRange(_viewModel.TimestampValue, _viewModel.PreviousTimestampValue, _viewModel.NextTimestampValue, -1, 1);
        else if (e.PropertyName == nameof(GlyphConfiguratorViewModel.SelectedGlyphOption))
            OnSelectedGlyphChanged();
    }

    private void BezierGraph_ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BezierGraphViewModel.CurrentY))
            _viewModel.Brightness = MathHelpers.ConvertNumberRange(BezierGraph.ViewModel.CurrentY, -1, 1, 0, 100);
        else if (e.PropertyName == nameof(BezierGraphViewModel.CurrentX))
            _viewModel.TimestampValue = BezierGraph.ViewModel.CurrentTimestamp;
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        await StorageManager.Instance.SaveAnimation(_animation);
        _ignoreUnsavedChanges = _hasUnsavedChanges = false;
    }

    private void OnSelectedInteroplationOptionChanged()
    {
        switch (_viewModel.SelectedInterpolationOption)
        {
            case EInterpolationType.Bezier:
                BezierGraph.IsEnabled = true;
                break;
            default:
                BezierGraph.IsEnabled = false;
                break;
        }
    }

    private void OnSelectedGlyphChanged()
    {
        FrameList.Clear();

        int keyframeCount = _animation.Keyframes[_viewModel.SelectedGlyphOption].Count > 0
            ? _animation.Keyframes[_viewModel.SelectedGlyphOption].Count
            : 1;
        for (int i = 0; i < keyframeCount; i++)
        {
            Button button = new();
            button.Background = Color.FromRgba(0, 0, 0, 0);
            button.Text = (i + 1).ToString();
            button.Clicked += (_, _) => OnSelectedFrameChanged(i);
            FrameList.Add(button);
        }

        OnSelectedFrameChanged(0); //TODO: Change this to selecte the closest frame to the previously selected one.
    }

    private void OnSelectedFrameChanged(int index)
    {
        SKeyframe keyframe = _animation.Keyframes[_viewModel.SelectedGlyphOption].ElementAtOrDefault(index);
        _viewModel.TimestampValue = keyframe.Timestamp;
        _viewModel.Brightness = MathHelpers.ConvertNumberRange(keyframe.Brightness, GlyphAPI.GLYPH_MIN_BRIGHTNESS, GlyphAPI.GLYPH_MAX_BRIGHTNESS, 0, 100);
        _viewModel.SelectedInterpolationOption = keyframe.Interpolation;
    }

    private void DeleteFrameButton_Clicked(object sender, EventArgs e)
    {
    }

    private void ShiftFrameLeftButton_Clicked(object sender, EventArgs e)
    {
    }

    private void ShiftFrameRightButton_Clicked(object sender, EventArgs e)
    {
    }

    /*private void InsertFrameLeftButton_Clicked(object sender, EventArgs e)
    {
    }*/

    private void InsertFrameRightButton_Clicked(object sender, EventArgs e)
    {
    }

    private void AddFrameButton_Clicked(object sender, EventArgs e)
    {
    }
}
