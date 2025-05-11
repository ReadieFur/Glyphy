using System.Reflection;
using Glyphy.Controls.AbsoluteLayoutExtensions;
using Timer = System.Timers.Timer;

namespace Glyphy.Controls.Bezier;

public partial class BezierGraph : ContentView
{
    public int FrameRate { get; set; } = 120;

    private record RDraggableElementNormalized(Point startPosition, PropertyInfo? xProp, PropertyInfo? yProp);
    private readonly Dictionary<VisualElement, RDraggableElementNormalized> _draggableElements = new();
    private readonly Timer _playbackTimer = new();
    private double _playbackT;

    public BezierGraph()
    {
        InitializeComponent();

        BezierGraphViewModel viewModel = new();
        BindingContext = viewModel;
        viewModel.PropertyChanged += BindingContext_PropertyChanged;

        _playbackT = viewModel.PreviousTimestamp;
        _playbackTimer.Elapsed += PlaybackTimer_Elapsed;
        _playbackTimer.Interval = 1000.0 / FrameRate;
        //Loaded += (_, _) => _playbackTimer.Start();
        Unloaded += (_, _) => _playbackTimer.Stop();
    }

    private void BindingContext_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is not BezierGraphViewModel viewModel)
            return;

        if (e.PropertyName == nameof(viewModel.CurrentInX))
        {
            //I would think this would cause a stack overflow by recusrion but I think the event under the hood prevents the update from being called on itself during an event dispatch.
            if (viewModel.CurrentInX > viewModel.CurrentX)
                viewModel.CurrentInX = viewModel.CurrentX;
        }

        if (e.PropertyName == nameof(viewModel.CurrentX))
        {
            //These have to be in this order.
            if (viewModel.CurrentX < viewModel.PreviousOutX)
                viewModel.CurrentX = viewModel.PreviousOutX;

            if (viewModel.CurrentInX > viewModel.CurrentX)
                viewModel.CurrentInX = viewModel.CurrentX;

            if (viewModel.CurrentX > viewModel.NextInX)
                viewModel.CurrentX = viewModel.NextInX;

            if (viewModel.CurrentOutX < viewModel.CurrentX)
                viewModel.CurrentOutX = viewModel.CurrentX;
        }

        if (e.PropertyName == nameof(viewModel.CurrentOutX))
        {
            if (viewModel.CurrentOutX < viewModel.CurrentX)
                viewModel.CurrentOutX = viewModel.CurrentX;
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

        double x = Helpers.ConvertNumberRange(_playbackT, t1, t2, x1, x2);
        double y = Helpers.BezierSolveYGivenX(p1, p2, p3, p4, x);

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
