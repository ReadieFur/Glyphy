using Glyphy.Misc;
using Microsoft.Maui.Controls.Shapes;

namespace Glyphy.Controls.AbsoluteLayoutExtensions
{
    public static class AbsoluteLayoutNormalizerLine
    {
        public static readonly BindableProperty X1Property = BindableProperty.CreateAttached("X1", typeof(double), typeof(AbsoluteLayoutNormalizer), double.NaN, propertyChanged: OnPropertyChanged);
        public static double GetX1(BindableObject view) => (double)view.GetValue(X1Property);
        public static void SetX1(BindableObject view, double value) => view.SetValue(X1Property, value);

        public static readonly BindableProperty Y1Property = BindableProperty.CreateAttached("Y1", typeof(double), typeof(AbsoluteLayoutNormalizer), double.NaN, propertyChanged: OnPropertyChanged);
        public static double GetY1(BindableObject view) => (double)view.GetValue(Y1Property);
        public static void SetY1(BindableObject view, double value) => view.SetValue(Y1Property, value);

        public static readonly BindableProperty X2Property = BindableProperty.CreateAttached("X2", typeof(double), typeof(AbsoluteLayoutNormalizer), double.NaN, propertyChanged: OnPropertyChanged);
        public static double GetX2(BindableObject view) => (double)view.GetValue(X2Property);
        public static void SetX2(BindableObject view, double value) => view.SetValue(X2Property, value);

        public static readonly BindableProperty Y2Property = BindableProperty.CreateAttached("Y2", typeof(double), typeof(AbsoluteLayoutNormalizer), double.NaN, propertyChanged: OnPropertyChanged);
        public static double GetY2(BindableObject view) => (double)view.GetValue(Y2Property);
        public static void SetY2(BindableObject view, double value) => view.SetValue(Y2Property, value);

        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue) => AbsoluteLayoutNormalizer.OnPropertyChanged<Line>(bindable, UpdatePosition);

        private static void UpdatePosition(Line line)
        {
            if (line.Parent is not AbsoluteLayout container)
                return;

            double canvasWidth = container.Width;
            double canvasHeight = container.Height;
            double x1 = GetX1(line);
            double y1 = GetY1(line);
            double x2 = GetX2(line);
            double y2 = GetY2(line);

            //Apply normalization: (Value + 1) * (CanvasWidth / 2) for X axis, and (1 - Value) * (CanvasHeight / 2) for Y axis.
            line.X1 = x1.IsValid() ? (x1 + 1) * canvasWidth / 2 : 0;
            line.Y1 = y1.IsValid() ? (1 - y1) * canvasHeight / 2 : 0;
            line.X2 = x2.IsValid() ? (x2 + 1) * canvasWidth / 2 : 0;
            line.Y2 = y2.IsValid() ? (1 - y2) * canvasHeight / 2 : 0;
        }
    }
}
