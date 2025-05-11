using Microsoft.Maui.Controls.Shapes;
using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace Glyphy.Controls.AbsoluteLayoutExtensions
{
    public static class AbsoluteLayoutNormalizerBezier
    {
        public static readonly BindableProperty P1Property = BindableProperty.CreateAttached("P1", typeof(Point), typeof(AbsoluteLayoutNormalizer), default(Point), propertyChanged: OnPropertyChanged);
        public static Point GetP1(BindableObject view) => (Point)view.GetValue(P1Property);
        public static void SetP1(BindableObject view, Point value) => view.SetValue(P1Property, value);

        public static readonly BindableProperty P2Property = BindableProperty.CreateAttached("P2", typeof(Point), typeof(AbsoluteLayoutNormalizer), default(Point), propertyChanged: OnPropertyChanged);
        public static Point GetP2(BindableObject view) => (Point)view.GetValue(P2Property);
        public static void SetP2(BindableObject view, Point value) => view.SetValue(P2Property, value);

        public static readonly BindableProperty P3Property = BindableProperty.CreateAttached("P3", typeof(Point), typeof(AbsoluteLayoutNormalizer), default(Point), propertyChanged: OnPropertyChanged);
        public static Point GetP3(BindableObject view) => (Point)view.GetValue(P3Property);
        public static void SetP3(BindableObject view, Point value) => view.SetValue(P3Property, value);

        public static readonly BindableProperty P4Property = BindableProperty.CreateAttached("P4", typeof(Point), typeof(AbsoluteLayoutNormalizer), default(Point), propertyChanged: OnPropertyChanged);
        public static Point GetP4(BindableObject view) => (Point)view.GetValue(P4Property);
        public static void SetP4(BindableObject view, Point value) => view.SetValue(P4Property, value);

        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue) => AbsoluteLayoutNormalizer.OnPropertyChanged<Path>(bindable, UpdatePosition);

        private static void UpdatePosition(Path path)
        {
            if (path.Parent is not AbsoluteLayout container)
                return;

            double canvasWidth = container.Width;
            double canvasHeight = container.Height;

            //Ensure that the Path.Data and PathGeometry exist.
            PathGeometry geometry = path.Data as PathGeometry ?? (PathGeometry)(path.Data = new PathGeometry());

            //Ensure that there is at least one PathFigure
            if (geometry.Figures.Count == 0)
                geometry.Figures.Add(new PathFigure());

            //Get the first PathFigure (create if it doesn't exist).
            PathFigure figure = geometry.Figures[0];

            //Ensure that the first segment is a BezierSegment.
            if (figure.Segments.Count == 0)
                figure.Segments.Add(new BezierSegment());

            if (figure.Segments[0] is BezierSegment bezier)
            {
                //Retrieve the attached property values.
                Point p1 = GetP1(path);
                Point p2 = GetP2(path);
                Point p3 = GetP3(path);
                Point p4 = GetP4(path);

                //Apply the normalization logic and set the BezierSegment points.
                bezier.Point1 = new Point((p2.X + 1) * canvasWidth / 2, (1 - p2.Y) * canvasHeight / 2);
                bezier.Point2 = new Point((p3.X + 1) * canvasWidth / 2, (1 - p3.Y) * canvasHeight / 2);
                bezier.Point3 = new Point((p4.X + 1) * canvasWidth / 2, (1 - p4.Y) * canvasHeight / 2);

                //Set the StartPoint of the PathFigure.
                figure.StartPoint = new Point((p1.X + 1) * canvasWidth / 2, (1 - p1.Y) * canvasHeight / 2);
            }
        }
    }
}
