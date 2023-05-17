using Microsoft.Maui.Controls;

namespace Glyphy;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new MainPage();
	}
}
