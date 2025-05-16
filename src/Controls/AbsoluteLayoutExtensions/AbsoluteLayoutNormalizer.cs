using Glyphy.Misc;

namespace Glyphy.Controls.AbsoluteLayoutExtensions
{
    public static class AbsoluteLayoutNormalizer
    {
        #region Base methods
        public delegate void UpdateDelegate<T>(T element) where T : VisualElement;

        /// <summary>
        /// Calls the respective UpdatePosition delegate if the specified element type is a direct child of an AbsoluteLayout object.
        /// </summary>
        /// <typeparam name="T">The element type to filter for.</typeparam>
        /// <param name="bindable">The bindable object being updated.</param>
        /// <param name="updatePositionCallback">The update position method to call.</param>
        public static void OnPropertyChanged<T>(BindableObject bindable, UpdateDelegate<T> updatePositionCallback) where T : VisualElement
        {
            if (bindable is not T element)
                return;

            EventHandler updateHandler = (_, _) =>
            {
                //if (element.Parent is not AbsoluteLayout container)
                updatePositionCallback(element);
            };

            EventHandler parentChangedHandler = (_, _) =>
            {
                if (element.Parent is not AbsoluteLayout container)
                    return;

                //element.ParentChanged -= parentChangedHandler;
                container.SizeChanged += updateHandler;

                if (element.IsLoaded)
                    updatePositionCallback(element);
            };

            element.Loaded += updateHandler;

            //First time this code is called the parent is often unset.
            if (element.Parent is null)
            {
                element.ParentChanged += parentChangedHandler;
            }
            else if (element.Parent is AbsoluteLayout container)
            {
                element.ParentChanged -= parentChangedHandler;
                container.SizeChanged += updateHandler;
            }

            if (element.IsLoaded)
                updatePositionCallback(element);
        }
        #endregion

        public static readonly BindableProperty XProperty = BindableProperty.CreateAttached("X", typeof(double), typeof(AbsoluteLayoutNormalizer), double.NaN, propertyChanged: OnPropertyChanged);
        public static double GetX(BindableObject view) => (double)view.GetValue(XProperty);
        public static void SetX(BindableObject view, double value) => view.SetValue(XProperty, value);

        public static readonly BindableProperty YProperty = BindableProperty.CreateAttached("Y", typeof(double), typeof(AbsoluteLayoutNormalizer), double.NaN, propertyChanged: OnPropertyChanged);
        public static double GetY(BindableObject view) => (double)view.GetValue(YProperty);
        public static void SetY(BindableObject view, double value) => view.SetValue(YProperty, value);

        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue) => OnPropertyChanged<VisualElement>(bindable, UpdatePosition);

        private static void UpdatePosition(VisualElement element)
        {
            if (element.Parent is not AbsoluteLayout container)
                return;

            double normalizedX = GetX(element);
            double normalizedY = GetY(element);

            double centerX = container.Width / 2;
            double centerY = container.Height / 2;

            double absoluteX = normalizedX.IsValid() ? centerX + normalizedX * centerX - element.Width / 2 : 0;
            double absoluteY = normalizedY.IsValid() ? centerY - normalizedY * centerY - element.Width / 2 : 0;

            //https://learn.microsoft.com/en-us/dotnet/maui/user-interface/layouts/absolutelayout?view=net-maui-9.0
            AbsoluteLayout.SetLayoutBounds(element, new(absoluteX, absoluteY, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
        }
    }
}
