namespace Glyphy
{
    /// <summary>
    /// Requires that the <see cref="Microsoft.Maui.Controls.Page"/> is on the navigation stack.
    /// </summary>
    internal interface IThemeChangeHandler
    {
        void RequestedThemeChanged(bool isDark);
    }
}
