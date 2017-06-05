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

            string slideOutIcon = Device.RuntimePlatform == Device.UWP ? "Assets/slideout.png" : "slideout.png";

            var settings = new ToolbarItem
            {
                Icon = slideOutIcon,
                Text = MainMenuResources.SettingsIconText,
                Command = new Command(this.ShowSettingsPage),
            };
            this.ToolbarItems.Add(settings);
            NavigationPage.SetTitleIcon(this, slideOutIcon);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BannerAd.IsVisible = false;
            BannerAd.IsVisible = true;
        }

        private void ShowSettingsPage()
        {
            this.Navigation.PushAsync(new SettingsPage());
        }
        
        // Red wine
        private void RedWine_Clicked(object sender, EventArgs e)
        {
            ShowRedWinePage();
        }

        private void ShowRedWinePage()
        {
            this.Navigation.PushAsync(new RedWinePage());
        }

        // White Wine
        private void WhiteWine_Clicked(object sender, EventArgs e)
        {
            ShowWhiteWinePage();
        }

        private void ShowWhiteWinePage()
        {
            this.Navigation.PushAsync(new WhiteWinePage());
        }

        // Whiskey
        private void Whiskey_Clicked(object sender, EventArgs e)
        {
            ShowWhiskeyPage();
        }

        private void ShowWhiskeyPage()
        {
            this.Navigation.PushAsync(new WhiskeyPage());
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
