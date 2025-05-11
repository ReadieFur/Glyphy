using System.Reflection;

namespace Glyphy
{
    internal static class Helpers
    {
        public static double ConvertNumberRange(double oldValue, double oldMin, double oldMax, double newMin, double newMax) =>
            ((oldValue - oldMin) * (newMax - newMin) / (oldMax - oldMin)) + newMin;

        public static PropertyInfo? GetBindingPropertySource(this Element element, BindableProperty property)
        {
            //So much reflection to get this working in MAUI, WPF was a bit more straightforward, this took a lot of digging through the object data in the debugger.
            if (typeof(BindableObject).GetField("_properties", BindingFlags.Instance | BindingFlags.NonPublic) is FieldInfo propertiesFieldInfo
                && propertiesFieldInfo.GetValue(element) is object properties
                && properties.GetType().GetProperty("Item") is PropertyInfo bindingEntries
                && bindingEntries.GetValue(properties, [property]) is object bindingEntry
                && bindingEntry.GetType().GetField("Bindings", BindingFlags.Instance | BindingFlags.Public) is FieldInfo bindingsFieldInfo
                && bindingsFieldInfo.GetValue(bindingEntry) is object bindings
                && bindings.GetType().GetMethod("GetValue", BindingFlags.Instance | BindingFlags.Public) is MethodInfo bindingMethodInfo
                && bindingMethodInfo.Invoke(bindings, []) is Binding binding
                && element.BindingContext.GetType().GetProperty(binding.Path) is PropertyInfo propertyInfo) //Binding context dosen't need to be passed as it is automatically inherited/set on the object.
                return propertyInfo;
            return null;
        }

        public static Point GetBoundsRelativeTo(this VisualElement view, VisualElement relativeTo)
        {
            Rect abs = view.Bounds;
            Rect rel = relativeTo.Bounds;
            return new(abs.X - rel.X, abs.Y - rel.Y);
        }

        public static double BezierSolveYGivenX(Point p0, Point p1, Point p2, Point p3, double xTarget)
        {
            double t = SolveBezierT(p0.X, p1.X, p2.X, p3.X, xTarget);
            double u = 1.0 - t;

            double y = u * u * u * p0.Y
                     + 3 * u * u * t * p1.Y
                     + 3 * u * t * t * p2.Y
                     + t * t * t * p3.Y;

            return y;
        }

        //Solves for t′ such that BezierX(t′) ≈ xTarget using binary search.
        private static double SolveBezierT(double x0, double x1, double x2, double x3, double xTarget, int maxIterations = 15, double epsilon = 0.0001)
        {
            double tLow = 0;
            double tHigh = 1;
            double t = 0.5;

            for (int i = 0; i < maxIterations; i++)
            {
                double u = 1 - t;
                double x = u * u * u * x0
                        + 3 * u * u * t * x1
                        + 3 * u * t * t * x2
                        + t * t * t * x3;

                if (Math.Abs(x - xTarget) < epsilon) break;

                if (x < xTarget)
                    tLow = t;
                else
                    tHigh = t;

                t = (tLow + tHigh) * 0.5f;
            }

            return t;
        }
    }
}
