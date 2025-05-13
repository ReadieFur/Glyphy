using System.Reflection;

namespace Glyphy.Misc
{
    internal static class MauiHelpers
    {
        public static PropertyInfo? GetBindingPropertySource(this Element element, BindableProperty property)
        {
            //So much reflection to get this working in MAUI, WPF was a bit more straightforward, this took a lot of digging through the object data in the debugger.
            if (typeof(BindableObject).GetField("_properties", BindingFlags.Instance | BindingFlags.NonPublic) is FieldInfo propertiesFieldInfo
                && propertiesFieldInfo.GetValue(element) is object properties
                && properties.GetType().GetProperty("Item") is PropertyInfo bindingEntries
                && bindingEntries.GetValue(properties, [property]) is object bindingEntry
                && bindingEntry.GetType().GetField("Bindings", BindingFlags.Instance | BindingFlags.Public) is FieldInfo bindingsFieldInfo
                && bindingsFieldInfo.GetValue(bindingEntry) is object bindings
#if NET8_0
                && bindings.GetType().GetMethod("GetValueAtIndex", BindingFlags.Instance | BindingFlags.Public) is MethodInfo bindingMethodInfo
                && bindingMethodInfo.Invoke(bindings, [0]) is Binding binding
#elif NET9_0
                && bindings.GetType().GetMethod("GetValue", BindingFlags.Instance | BindingFlags.Public) is MethodInfo bindingMethodInfo
                && bindingMethodInfo.Invoke(bindings, []) is Binding binding
#endif
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
    }
}
