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

    #region Other bindings
    public static readonly BindableProperty FrameRateProperty = CreateBinding(BindingMode.OneWay, 120);
    public int FrameRate { get => (int)GetValue(FrameRateProperty); set => SetValue(FrameRateProperty, value); }
    #endregion

    //Expose ViewModel.
    public BezierGraphViewModel ViewModel { get => (BezierGraphViewModel)BindingContext; set => BindingContext = value; }

    private record RDraggableElementNormalized(Point startPosition, PropertyInfo? xProp, PropertyInfo? yProp);
    private readonly Dictionary<VisualElement, RDraggableElementNormalized> _draggableElements = new();
    private readonly Timer _playbackTimer = new();
    private double _playbackT;

    public BezierGraph()
    {
        InitializeComponent();

        if (BindingContext is null)
            ViewModel = new();
        else if (BindingContext is not BezierGraphViewModel)
            throw new InvalidOperationException();

        _playbackT = ViewModel.PreviousTimestamp;
        _playbackTimer.Elapsed += PlaybackTimer_Elapsed;
        _playbackTimer.Interval = 1000.0 / FrameRate;
        Unloaded += (_, _) => _playbackTimer.Dispose();
    }

    private void OnOuterPropertyChanged(string propertyName, object oldValue, object newValue)
    {
    }

    private void PlaybackTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        double frameInterval = 1000.0 / FrameRate;

        _playbackTimer.Interval = frameInterval;
        _playbackT += frameInterval;

        if (_playbackT > ViewModel.NextTimestamp) //Not using >= as we want to over-run by one frame.
            _playbackT = ViewModel.PreviousTimestamp;

        double t1, t2, x1, x2;
        Point p1, p2, p3, p4;
        if (_playbackT < ViewModel.CurrentTimestamp)
        {
            //Animate along the first bezier.
            t1 = ViewModel.PreviousTimestamp;
            t2 = ViewModel.CurrentTimestamp;
            x1 = ViewModel.PreviousX;
            x2 = ViewModel.CurrentX;
            p1 = ViewModel.PreviousPoint;
            p2 = ViewModel.PreviousOutTangent;
            p3 = ViewModel.CurrentInTangent;
            p4 = ViewModel.CurrentPoint;
        }
        else
        {
            //Animate along the second bezier.
            t1 = ViewModel.CurrentTimestamp;
            t2 = ViewModel.NextTimestamp;
            x1 = ViewModel.CurrentX;
            x2 = ViewModel.NextX;
            p1 = ViewModel.CurrentPoint;
            p2 = ViewModel.CurrentOutTangent;
            p3 = ViewModel.NextInTangent;
            p4 = ViewModel.NextPoint;
        }

        double x = MathHelpers.ConvertNumberRange(_playbackT, t1, t2, x1, x2);
        double y = MathHelpers.BezierSolveYGivenX(p1, p2, p3, p4, x);

        try
        {
            Dispatcher.Dispatch(() =>
            {
                ViewModel.PlayheadX = x;
                ViewModel.PlayheadY = y;
            });
        }
        catch (TaskCanceledException) { } //Ignore this error.
    }

    private void DraggableElementNormalized_OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (sender is not VisualElement element || element.BindingContext is not BezierGraphViewModel viewModel || element.Parent is not AbsoluteLayout container)
            return;

        //https://github.com/xamarin/Xamarin.Forms/issues/3469
        //https://stackoverflow.com/questions/39458078/xamarin-pan-gesture-recognizer-not-giving-accurate-coordinates/54664267#54664267
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
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

                        /* On android we need to:
                         * Set the start point as the center position of the container
                         * And then offset this by the width of the element / 2
                         * (idk in my head this made sense and works, how I don't know to be honest, I'm horrible at maths).
                         */
                        double xContainerCenter = container.X + (container.Width / 2);
                        double xElementHalfWidth = element.Width / 2;
                        double xStartPosition = xContainerCenter - xElementHalfWidth;

                        double yContainerCenter = container.Y + (container.Height / 2);
                        double yElementHalfWidth = container.Height / 2;
                        double yStartPosition = yContainerCenter - yElementHalfWidth;

                        _draggableElements[element] = _draggableElements[element] with
                        {
                            startPosition = new(
                                xStartPosition,
                                yStartPosition
                            )
                        };
                    }
                    break;
                case GestureStatus.Running:
                    {
                        if (!_draggableElements.TryGetValue(element, out RDraggableElementNormalized? info))
                            return;

                        if (info.xProp is PropertyInfo xProp)
                        {
                            double absoluteX = (e.TotalX + element.X) - info.startPosition.X;
                            double normalizedX = absoluteX / (container.Width / 2);
                            xProp.SetValue(ViewModel, Math.Clamp(normalizedX, -1, 1));
                        }

                        if (info.yProp is PropertyInfo yProp)
                        {
                            double absoluteY = -((e.TotalY + element.Y) - info.startPosition.Y);
                            double normalizedY = absoluteY / (container.Height / 2);
                            yProp.SetValue(ViewModel, Math.Clamp(normalizedY, -1, 1));
                        }
                    }
                    break;
                case GestureStatus.Completed:
                    break;
            }
        }
        else
        {
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
                    }
                    break;
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
                    }
                    break;
                case GestureStatus.Completed:
                    break;
            }
        }
    }
}
