using Vaerator.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

//[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Vaerator
{
	public partial class App : Application
	{
        public App()
		{
			InitializeComponent();
			SetMainPage();
		}

		public static void SetMainPage()
		{
            var mainMenuPage = new NavigationPage(new MainMenuPage());
            Current.MainPage = mainMenuPage;
        }
    }
}
