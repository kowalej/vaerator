using Localization;
using Localization.Localize;
using Localization.TranslationResources;
using System;
using Vaerator.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Vaerator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : BasePage
    {
        public SettingsPage()
        {
			InitializeComponent();

            // Need to use standard picker on UWP desktop
            if (Device.RuntimePlatform == Device.UWP && Device.Idiom == TargetIdiom.Desktop)
            {
                LanguagePicker.IsVisible = true;
                LanguagePickerSelectionLabel.IsVisible = false;
            }
        }

        private void LanguagePickerCell_OnTapped(object sender, EventArgs e)
        {
            if (!(Device.RuntimePlatform == Device.UWP && Device.Idiom == TargetIdiom.Desktop)) 
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    LanguagePicker.Focus();
                });
            }
        }

        private void LanguagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            // RefreshCulture(); - Done in ViewModel when binding new language picker index.
            SetTranslationText();
        }

        private void BackgroundSimEnabledCell_OnTapped(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                BackgroundSimEnabledSwitch.IsToggled = BackgroundSimEnabledSwitch.IsToggled ? false : true;
            });
        }

        private void HowToUseCell_OnTapped(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(SettingsResources.HowToUseURL));
        }

        private void AboutAerationCell_OnTapped(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(SettingsResources.AboutAerationURL));
        }

        private void ContactUsCell_OnTapped(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(SettingsResources.ContactUsURL));
        }

        private void CompanyInfoCell_OnTapped(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(SettingsResources.CompanyHomeURL));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        string AddLineBreakURL(string text)
        {
            return text.Insert(text.IndexOf("("), "\n");
        }

        protected override void SetTranslationText()
        {
            Title = SettingsResources.SettingsPageTitle;
            SettingsSection.Title = SettingsResources.SettingsSectionTitle;
            LanguagePickerLabel.Text = SettingsResources.LanguagePickerLabel;
            BackgroundSimEnabledLabel.Text = SettingsResources.BackgroundSimEnabledLabel;
            InformationSection.Title = SettingsResources.InformationSectionTitle;
            HowToUseLabel.Text = SettingsResources.HowToUse;
            AboutAerationLabel.Text = SettingsResources.AboutAeration;
            ContactUsLabel.Text = SettingsResources.ContactUs;
            //CompanyHomeLabel.Text = SettingsResources.CompanyHome;
        }
    }
}
