using Localization.TranslationResources;
using System;
using Xamarin.Forms;

namespace Vaerator.Views
{
	public partial class MainMenuPage : BasePage
    {
		public MainMenuPage ()
		{
            InitializeComponent();
            string slideOutIcon = Device.RuntimePlatform == Device.Windows ? "Assets/slideout.png" : "slideout.png";

            var settings = new ToolbarItem
            {
                Icon = slideOutIcon,
                Text = MainMenuResources.SettingsIconText,
                Command = new Command(this.ShowSettingsPage),
            };
            this.ToolbarItems.Add(settings);
            NavigationPage.SetTitleIcon(this, slideOutIcon);
        }

        private void ShowSettingsPage()
        {
            this.Navigation.PushAsync(new SettingsPage());
        }

        private void RedWine_Clicked(object sender, EventArgs e)
        {
            ShowRedWinePage();
        }

        private void ShowRedWinePage()
        {
            this.Navigation.PushAsync(new RedWinePage());
        }

        /*async void OnPreviousPageButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        async void OnRootPageButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }*/
    }
}
