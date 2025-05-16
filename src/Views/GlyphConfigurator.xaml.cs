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
    private SAnimation _animation;

	public GlyphConfigurator()
	{
		InitializeComponent();
        Setup();
        _animation = new();

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

    public GlyphConfigurator(Guid animationId)
    {
        InitializeComponent();
        Setup();

        switch (_viewModel.SelectedInterpolationOption)
        {
            case EInterpolationType.Bezier:
                BezierGraph.IsEnabled = true;
                break;
            default:
                BezierGraph.IsEnabled = false;
                break;
        }

        IsEnabled = false;

        try
        {
            StorageManager.Instance.LoadAnimation(animationId).ContinueWith(t =>
            {
                _animation = t.Result;

                if (_animation.Keyframes.Count > 0)
                {
                    //This will trigger the UI to load the rest of the elements for this keyframe, no need to do anything else here.
                    _viewModel.SelectedGlyphOption = _animation.Keyframes.First().Key;
                }

                IsEnabled = true;
            }).ConfigureAwait(true);
        }
        catch (FileNotFoundException ex)
        {
            Toast.Make(ex.Message, CommunityToolkit.Maui.Core.ToastDuration.Long);
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
        await Navigation.PopAsync();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GlyphConfiguratorViewModel.SelectedInterpolationOption))
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
        else if (e.PropertyName == nameof(GlyphConfiguratorViewModel.Brightness))
            BezierGraph.ViewModel.CurrentY = MathHelpers.ConvertNumberRange(_viewModel.Brightness, 0, 100, -1, 1);
        else if (e.PropertyName == nameof(GlyphConfiguratorViewModel.TimestampValue))
            BezierGraph.ViewModel.CurrentX = MathHelpers.ConvertNumberRange(_viewModel.TimestampValue, _viewModel.PreviousTimestampValue, _viewModel.NextTimestampValue, -1, 1);
    }

    private void BezierGraph_ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BezierGraphViewModel.CurrentY))
            _viewModel.Brightness = MathHelpers.ConvertNumberRange(BezierGraph.ViewModel.CurrentY, -1, 1, 0, 100);
        else if (e.PropertyName == nameof(BezierGraphViewModel.CurrentX))
            _viewModel.TimestampValue = BezierGraph.ViewModel.CurrentTimestamp;
    }

    private void SaveButton_Clicked(object sender, EventArgs e)
    {
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

    private void InsertFrameLeftButton_Clicked(object sender, EventArgs e)
    {
    }

    private void InsertFrameRightButton_Clicked(object sender, EventArgs e)
    {
    }

    private void AddFrameButton_Clicked(object sender, EventArgs e)
    {
    }
}
