using System.Reflection;
using System.Runtime.CompilerServices;
using Glyphy.Controls.AbsoluteLayoutExtensions;
using Glyphy.Misc;
using Timer = System.Timers.Timer;

namespace Glyphy.Controls.Bezier;

public partial class BezierGraph : ContentView
{
    private static BindableProperty CreateBinding<T>(BindingMode bindingMode, T? defaultValue = default, [CallerMemberName] string? name = null)
    {
        if (name is null || !name.EndsWith("Property"))
            throw new ArgumentException(nameof(name));

        string propertyName = name.Substring(0, name.Length - "Property".Length);

        BindableProperty.BindingPropertyChangedDelegate onChanged = (bindable, oldValue, newValue) =>
        {
            if (bindable is not BezierGraph self)
                return;
            self.OnOuterPropertyChanged(propertyName, oldValue, newValue);
        };

        return BindableProperty.Create(propertyName, typeof(T), typeof(BezierGraph), defaultValue, bindingMode, propertyChanged: onChanged);
    }

    #region Outer bindings
    public static readonly BindableProperty FrameRateProperty = CreateBinding(BindingMode.OneWay, 120);
    public int FrameRate { get => (int)GetValue(FrameRateProperty); set => SetValue(FrameRateProperty, value); }

    //Previous.
    public static readonly BindableProperty PreviousProperty = CreateBinding<Point>(BindingMode.TwoWay, new(-1, -1));
    public Point Previous { get => (Point)GetValue(PreviousProperty); set => SetValue(PreviousProperty, value); }

    public static readonly BindableProperty PreviousOutProperty = CreateBinding<Point>(BindingMode.TwoWay, new(-0.5, -1));
    public Point PreviousOut { get => (Point)GetValue(PreviousOutProperty); set => SetValue(PreviousOutProperty, value); }

    //Current.
    public static readonly BindableProperty CurrentInProperty = CreateBinding<Point>(BindingMode.TwoWay, new(-0.5, 0));
    public Point CurrentIn { get => (Point)GetValue(CurrentInProperty); set => SetValue(CurrentInProperty, value); }

    public static readonly BindableProperty CurrentProperty = CreateBinding<Point>(BindingMode.TwoWay, new(0, 0));
    public Point Current { get => (Point)GetValue(CurrentProperty); set => SetValue(CurrentProperty, value); }

    public static readonly BindableProperty CurrentOutProperty = CreateBinding<Point>(BindingMode.TwoWay, new(0.5, 0));
    public Point CurrentOut { get => (Point)GetValue(CurrentOutProperty); set => SetValue(CurrentOutProperty, value); }

    //Next.
    public static readonly BindableProperty NextInProperty = CreateBinding<Point>(BindingMode.TwoWay, new(0.5, 1));
    public Point NextIn { get => (Point)GetValue(NextInProperty); set => SetValue(NextInProperty, value); }

    public static readonly BindableProperty NextProperty = CreateBinding<Point>(BindingMode.TwoWay, new(1, 1));
    public Point Next { get => (Point)GetValue(NextProperty); set => SetValue(NextProperty, value); }
    #endregion

    private record RDraggableElementNormalized(Point startPosition, PropertyInfo? xProp, PropertyInfo? yProp);
    private readonly BezierGraphViewModel _viewModel = new();
    private readonly Dictionary<VisualElement, RDraggableElementNormalized> _draggableElements = new();
    private readonly Timer _playbackTimer = new();
    private double _playbackT;

    public BezierGraph()
    {
        InitializeComponent();

        BindingContext = _viewModel;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;

        _playbackT = _viewModel.PreviousTimestamp;
        _playbackTimer.Elapsed += PlaybackTimer_Elapsed;
        _playbackTimer.Interval = 1000.0 / FrameRate;
        Unloaded += (_, _) => _playbackTimer.Dispose();
    }

    private void OnOuterPropertyChanged(string propertyName, object oldValue, object newValue)
    {
        //typeof(BezierGraphViewModel).GetProperty(nameof(BezierGraphViewModel.CurrentPoint))!.SetValue(_viewModel, Current);
        switch (propertyName)
        {
            //I don't need to check if the values match (preventing a circular update) because this happens internally on the view model.
            //Previous.
            case nameof(Previous):
                _viewModel.PreviousPoint = Previous;
                break;
            case nameof(PreviousOut):
                _viewModel.PreviousOutTangent = PreviousOut;
                break;
            //Current.
            case nameof(CurrentIn):
                _viewModel.CurrentInTangent = CurrentIn;
                break;
            case nameof(Current):
                _viewModel.CurrentPoint = Current;
                break;
            case nameof(CurrentOut):
                _viewModel.CurrentOutTangent = CurrentOut;
                break;
            //Next.
            case nameof(NextIn):
                _viewModel.NextInTangent = NextIn;
                break;
            case nameof(Next):
                _viewModel.NextPoint = NextIn;
                break;
            default:
                return;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            //Previous.
            case nameof(BezierGraphViewModel.PreviousPoint):
                Previous = _viewModel.PreviousPoint;
                break;
            case nameof(BezierGraphViewModel.PreviousOutTangent):
                Previous = _viewModel.PreviousOutTangent;
                break;
            //Current.
            case nameof(BezierGraphViewModel.CurrentInTangent):
                CurrentIn = _viewModel.CurrentInTangent;
                break;
            case nameof(BezierGraphViewModel.CurrentPoint):
                Current = _viewModel.CurrentPoint;
                break;
            case nameof(BezierGraphViewModel.CurrentOutTangent):
                CurrentOut = _viewModel.CurrentOutTangent;
                break;
            //Next.
            case nameof(BezierGraphViewModel.NextInTangent):
                NextIn = _viewModel.NextInTangent;
                break;
            case nameof(BezierGraphViewModel.NextPoint):
                Next = _viewModel.NextPoint;
                break;
            default:
                return;
        }
    }

    private void PlaybackTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (BindingContext is not BezierGraphViewModel viewModel)
            return;

        double frameInterval = 1000.0 / FrameRate;

        _playbackTimer.Interval = frameInterval;
        _playbackT += frameInterval;

        if (_playbackT > viewModel.NextTimestamp) //Not using >= as we want to over-run by one frame.
            _playbackT = viewModel.PreviousTimestamp;

        double t1, t2, x1, x2;
        Point p1, p2, p3, p4;
        if (_playbackT < viewModel.CurrentTimestamp)
        {
            //Animate along the first bezier.
            t1 = viewModel.PreviousTimestamp;
            t2 = viewModel.CurrentTimestamp;
            x1 = viewModel.PreviousX;
            x2 = viewModel.CurrentX;
            p1 = viewModel.PreviousPoint;
            p2 = viewModel.PreviousOutTangent;
            p3 = viewModel.CurrentInTangent;
            p4 = viewModel.CurrentPoint;
        }
        else
        {
            //Animate along the second bezier.
            t1 = viewModel.CurrentTimestamp;
            t2 = viewModel.NextTimestamp;
            x1 = viewModel.CurrentX;
            x2 = viewModel.NextX;
            p1 = viewModel.CurrentPoint;
            p2 = viewModel.CurrentOutTangent;
            p3 = viewModel.NextInTangent;
            p4 = viewModel.NextPoint;
        }

        double x = MathHelpers.ConvertNumberRange(_playbackT, t1, t2, x1, x2);
        double y = MathHelpers.BezierSolveYGivenX(p1, p2, p3, p4, x);

        try
        {
            Dispatcher.Dispatch(() =>
            {
                viewModel.PlayheadX = x;
                viewModel.PlayheadY = y;
            });
        }
        catch (TaskCanceledException) { } //Ignore this error.
    }

    private void DraggableElementNormalized_OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (sender is not VisualElement element || element.BindingContext is not BezierGraphViewModel viewModel || element.Parent is not AbsoluteLayout container)
            return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                {
                    //Get binding data on first run.
                    if (!_draggableElements.ContainsKey(element))
                        _draggableElements[element] = new RDraggableElementNormalized(
                            startPosition: default,
                            xProp: element.GetBindingPropertySource(AbsoluteLayoutNormalizer.XProperty),
                            yProp: element.GetBindingPropertySource(AbsoluteLayoutNormalizer.YProperty)
                        );

                    _draggableElements[element] = _draggableElements[element] with
                    {
                        startPosition = new(
                            _draggableElements[element].xProp?.GetValue(viewModel) as double? ?? 0.0,
                            _draggableElements[element].yProp?.GetValue(viewModel) as double? ?? 0.0
                        )
                    };
                    return;
                }
            case GestureStatus.Running:
                {
                    if (!_draggableElements.TryGetValue(element, out RDraggableElementNormalized? info))
                        return;

                    if (info.xProp is PropertyInfo xProp)
                    {
                        double xNorm = e.TotalX / (container.Width / 2); //Calculate the total distance moved in normalized space.
                        xNorm += info.startPosition.X; //Offset by the start coordinate.
                        xProp.SetValue(viewModel, Math.Clamp(xNorm, -1, 1));
                    }

                    if (info.yProp is PropertyInfo yProp)
                    {
                        double yNorm = -(e.TotalY / (container.Height / 2)); //Y is inverted because our coordinate space goes from bottom-to-top instead of the default top-to-bottom.
                        yNorm += info.startPosition.Y;
                        yProp.SetValue(viewModel, Math.Clamp(yNorm, -1, 1));
                    }

                    return;
                }
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
            default:
                return;
        }
    }
}
