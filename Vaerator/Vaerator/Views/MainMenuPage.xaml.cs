using Localization.TranslationResources;
using Plugin.Vibrate;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void SetTranslationText()
        {
            Title = MainMenuResources.MainMenuPageTitle;
            RedWineButton.Text = MainMenuResources.AerateRedWine;
            WhiteWineButton.Text = MainMenuResources.AerateWhiteWine;
            WhiskeyButton.Text = MainMenuResources.AerateWhiskey;
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
