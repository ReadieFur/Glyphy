namespace Glyphy
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

#if ANDROID
            Current!.RequestedThemeChanged += Current_RequestedThemeChanged;
#endif

#if DEBUG && true
            MainPage = new Views.TestPage();
#else
            MainPage = new Views.MainPage();
#endif
        }
    }
}
