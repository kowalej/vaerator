using Localization.TranslationResources;
using System;
using System.Threading.Tasks;
using Vaerator.Misc;
using Xamarin.Forms;

namespace Vaerator.Views
{
	public partial class MainMenuPage : BasePage
    {
        bool isPaging = false;
        object pagingLock = new object();
        readonly int DELAY_PRESS = 500;

		public MainMenuPage ()
		{
            InitializeComponent();
            #if DEBUG
                BannerAd.AdUnitID = Device.RuntimePlatform == Device.UWP ? UsefulStuff.UWPTest_BannerAdUnitID : UsefulStuff.AdMobTest_BannerAdUnitID;
            #else
                BannerAd.AdUnitID = Device.RuntimePlatform == Device.UWP ? UsefulStuff.UWP_BannerAdUnitID : UsefulStuff.AdMob_BannerAdUnitID;
            #endif
            string slideOutIcon = Device.RuntimePlatform == Device.UWP ? "Assets/slideout.png" : "slideout.png";
            var settings = new ToolbarItem
            {
                Icon = slideOutIcon,
                Text = MainMenuResources.SettingsIconText,
                Command = new Command(async () => { await this.ShowSettingsPage(); }),
            };
            this.ToolbarItems.Add(settings);
            //NavigationPage.SetTitleIcon(this, slideOutIcon);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Device.RuntimePlatform == Device.UWP)
            {
                BannerAd.IsVisible = false;
                BannerAd.IsVisible = true;
            }
        }

        private async Task ShowSettingsPage()
        {
            lock (pagingLock)
            {
                if (isPaging)
                    return;
                isPaging = true;
            }
            await this.Navigation.PushAsync(new SettingsPage());
            isPaging = false;
        }

        // Red wine
        private async void RedWine_Clicked(object sender, EventArgs e)
        {
            await ShowRedWinePage();
        }

        private async Task ShowRedWinePage()
        {
            lock (pagingLock)
            {
                if (isPaging)
                    return;
                isPaging = true;
            }
            await this.Navigation.PushAsync(new RedWinePage());
            await Task.Delay(DELAY_PRESS);
            lock (pagingLock)
            {
                isPaging = false;
            }
        }

        // White Wine
        private async void WhiteWine_Clicked(object sender, EventArgs e)
        {
            await ShowWhiteWinePage();
        }

        private async Task ShowWhiteWinePage()
        {
            lock (pagingLock)
            {
                if (isPaging)
                    return;
                isPaging = true;
            }
            await this.Navigation.PushAsync(new WhiteWinePage());
            await Task.Delay(DELAY_PRESS);
            lock (pagingLock)
            {
                isPaging = false;
            }
        }

        // Whiskey
        private async void Whiskey_Clicked(object sender, EventArgs e)
        {
            await ShowWhiskeyPage();
        }

        private async Task ShowWhiskeyPage()
        {
            lock (pagingLock)
            {
                if (isPaging)
                    return;
                isPaging = true;
            }
            await this.Navigation.PushAsync(new WhiskeyPage());
            await Task.Delay(DELAY_PRESS);
            lock (pagingLock)
            {
                isPaging = false;
            }
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
