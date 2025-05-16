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
            //MainPage = new Views.TestPage();
            MainPage = new NavigationPage(new Views.GlyphConfigurator());
#else
            MainPage = new NavigationPage(new Views.MainPage());
#endif
        }
    }
}
