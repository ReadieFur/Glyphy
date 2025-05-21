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
            //MainPage = new NavigationPage(new Views.GlyphConfigurator(Guid.Parse("8be015c8-8df4-47e2-be17-99be464dc9da")));
            MainPage = new Views.TestPage2();
#else
            MainPage = new NavigationPage(new Views.MainPage());
#endif
        }
    }
}
